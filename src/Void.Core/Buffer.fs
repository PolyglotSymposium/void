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

    let cursorMoved buffer newPosition =
        let event = CursorMoved(buffer.CursorPosition, newPosition)
        { buffer with CursorPosition = newPosition }, event

    let private setCursorPosition buffer newPosition =
        if buffer.CursorPosition = newPosition
        then buffer, DidNotMove
        else cursorMoved buffer newPosition

    let handleMoveCursorByRows buffer (moveCursor : MoveCursor<By.Row>) =
        match moveCursor with
        | MoveCursor (Move.Backward (By.Row rows)) ->
            let rowsToMove =
                if buffer.CursorPosition.Row >= rows
                then rows
                else buffer.CursorPosition.Row
            if rowsToMove > 0<mRow>
            then
                above buffer.CursorPosition rowsToMove
                |> cursorMoved buffer
            else buffer, DidNotMove
        | MoveCursor (Move.Forward (By.Row rows)) ->
            let rowsToTheEnd = lengthInRows buffer - buffer.CursorPosition.Row
            let rowsToMove =
                if rowsToTheEnd > rows
                then rows
                else rowsToTheEnd - 1<mRow>
            if rowsToMove > 0<mRow>
            then
                below buffer.CursorPosition rowsToMove
                |> cursorMoved buffer
            else buffer, DidNotMove

    let handleMoveCursorByColumns buffer (moveCursor : MoveCursor<By.Column>) =
        match moveCursor with
        | MoveCursor (Move.Backward (By.Column columns)) ->
            buffer, DidNotMove
        | MoveCursor (Move.Forward (By.Column columns)) ->
            buffer, DidNotMove

    let handleMoveCursorToLineInBuffer buffer (moveCursorTo : MoveCursorTo<By.Line, In.Buffer>) =
        match moveCursorTo with
        | MoveCursorTo MoveTo.First ->
            setCursorPosition buffer originCell
        | MoveCursorTo (MoveTo.Nth (By.Line line)) ->
            ``line#->row#`` line
            |> below originCell
            |> setCursorPosition buffer
        | MoveCursorTo MoveTo.Last ->
            below originCell (lengthInRows buffer - 1<mRow>)
            |> setCursorPosition buffer

    let handleMoveCursorToRowInBuffer buffer (moveCursorTo : MoveCursorTo<By.Row, In.Buffer>) =
        match moveCursorTo with
        | MoveCursorTo MoveTo.First ->
            setCursorPosition buffer originCell
        | MoveCursorTo (MoveTo.Nth (By.Row row)) ->
            below originCell row
            |> setCursorPosition buffer
        | MoveCursorTo MoveTo.Last ->
            below originCell (lengthInRows buffer - 1<mRow>)
            |> setCursorPosition buffer
