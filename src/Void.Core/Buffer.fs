namespace Void.Core

type FileBuffer = private {
    // TODO This is naive, obviously
    Filepath : string option
    Contents : string list
    CursorPosition : CellGrid.Cell
}

module Buffer =
    open CellGrid

    let lengthInRows (buffer : FileBuffer) =
        buffer.Contents.Length * 1<mRow>

    let emptyFile =
        { Filepath = None; Contents = []; CursorPosition = originCell }

    let loadContents (buffer : FileBuffer) contents =
        { buffer with Contents = contents }

    let prepend (buffer : FileBuffer) line =
        { buffer with Contents = line :: buffer.Contents }

    let newFile path =
        { Filepath = Some path; Contents = []; CursorPosition = originCell }

    let existingFile path contents =
        { Filepath = Some path; Contents = contents; CursorPosition = originCell }

    let readLines fileBuffer start =
        fileBuffer.Contents |> Seq.skip ((start - 1<mLine>)/1<mLine>) // Line numbers start at 1

    type CursorEvent =
        | DidNotMove
        | CursorMoved of From : Cell * To : Cell

    let handleMoveCursorByRows buffer (moveCursor : MoveCursor<ByRow, mRow>) =
        match moveCursor with
        | MoveCursor (Move.Backward rows) ->
            let rowsToMove =
                if buffer.CursorPosition.Row >= rows
                then rows
                else buffer.CursorPosition.Row
            if rowsToMove > 0<mRow>
            then
                let newPosition = above buffer.CursorPosition rowsToMove
                let event = CursorMoved(buffer.CursorPosition, newPosition)
                { buffer with CursorPosition = newPosition }, event
            else buffer, DidNotMove
        | MoveCursor (Move.Forward rows) ->
            let rowsToMove =
                if lengthInRows buffer > rows
                then rows
                else lengthInRows buffer - 1<mRow>
            if rowsToMove > 0<mRow>
            then
                let newPosition = below buffer.CursorPosition rowsToMove
                let event = CursorMoved(buffer.CursorPosition, newPosition)
                { buffer with CursorPosition = newPosition }, event
            else buffer, DidNotMove

    let handleMoveCursorByColumns buffer (moveCursor : MoveCursor<ByColumn, mColumn>) =
        match moveCursor with
        | MoveCursor (Move.Backward columns) ->
            buffer, DidNotMove
        | MoveCursor (Move.Forward columns) ->
            buffer, DidNotMove

