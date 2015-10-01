namespace Void.ViewModel

module RenderWindows =
    open Void.Core
    open Void.Core.CellGrid

    let private textLineAsDrawingObject x line =
        DrawingObject.Text {
            Text = line
            UpperLeftCorner = GridConvert.upperLeftCornerOf <| below originCell (x * 1<mRow>)
            Color = Colors.defaultColorscheme.Foreground
        }

    let textLinesAsDrawingObjects =
        List.mapi textLineAsDrawingObject

    let contentsAsDrawingObjects dimensions (buffer : string list) =
        let background = DrawingObject.Block {
            Area = GridConvert.boxAround { UpperLeftCell = originCell; Dimensions = dimensions }
            Color = Colors.defaultColorscheme.Background
        }

        let bufferLines = textLinesAsDrawingObjects buffer

        let rowsNotInBuffer =
            let lineNotInBufferAsDrawingObject i =
                DrawingObject.Text {
                    Text = "~"
                    UpperLeftCorner = GridConvert.upperLeftCornerOf { Row = i * 1<mRow>; Column = 0<mColumn> }
                    Color = Colors.defaultColorscheme.DimForeground
                }
            let linesWithNoTilde =
                if bufferLines.Length = 0
                then 1
                else bufferLines.Length
            [linesWithNoTilde..(dimensions.Rows / 1<mRow> - 1)]
            |> List.map lineNotInBufferAsDrawingObject

        List.append (background :: bufferLines) rowsNotInBuffer

    let private getCharacter (buffer : string list) cell =
        buffer.[cell.Row/1<mRow>].[cell.Column/1<mColumn>].ToString()

    let cursorAsDrawingObjects (window : WindowView) =
        match window.Cursor with
        | Visible cursor ->
            match cursor with
            | CursorView.Block cell ->
                DrawingObject.Block {
                    Area = GridConvert.boxAroundOneCell cell
                    Color = Colors.defaultColorscheme.Foreground
                } :: if window.Buffer.Length > 0
                     then [DrawingObject.Text {
                        Text = getCharacter window.Buffer cell
                        UpperLeftCorner = GridConvert.upperLeftCornerOf cell
                        Color = Colors.defaultColorscheme.Background
                     }]
                     else []
            | CursorView.IBeam _ -> []
        | Hidden -> []

    let windowAsDrawingObjects (window : WindowView) =
        List.append (contentsAsDrawingObjects window.Dimensions window.Buffer) (cursorAsDrawingObjects window)

    let private renderWindow window =
        let drawings = windowAsDrawingObjects window
        VMEvent.ViewPortionRendered(GridConvert.boxAround { UpperLeftCell = originCell; Dimensions = window.Dimensions }, drawings) :> Message

    let asDrawingObjects (windows : WindowView list) =
        [
            windowAsDrawingObjects windows.[0]
        ] |> Seq.concat

    let handleWindowEvent =
        function
        | Window.Event.ContentsUpdated window ->
            renderWindow window
        | Window.Event.Initialized window ->
            renderWindow window

    let handleWindowCommand (Window.Command.RedrawWindow window) =
        renderWindow window

    module Service =
        let subscribe (bus : Bus) =
            handleWindowEvent |> bus.subscribe
            handleWindowCommand |> bus.subscribe