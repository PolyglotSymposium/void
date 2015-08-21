namespace Void.ViewModel

module RenderWindows =
    open Void.Core
    open Void.Core.CellGrid

    let private textLineAsDrawingObject x line =
        DrawingObject.Text {
            Text = line
            UpperLeftCorner = GridConvert.upperLeftCornerOf <| below originCell x
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
                    UpperLeftCorner = GridConvert.upperLeftCornerOf { Row = i; Column = 0 }
                    Color = Colors.defaultColorscheme.DimForeground
                }
            let linesWithNoTilde =
                if bufferLines.Length = 0
                then 1
                else bufferLines.Length
            [linesWithNoTilde..dimensions.Rows-1]
            |> List.map lineNotInBufferAsDrawingObject

        List.append (background :: bufferLines) rowsNotInBuffer

    let private renderWindow (window : WindowView) =
        let drawings = contentsAsDrawingObjects window.Dimensions window.Buffer
        VMEvent.ViewPortionRendered(GridConvert.boxAround { UpperLeftCell = originCell; Dimensions = window.Dimensions }, drawings) :> Message

    let asDrawingObjects (windows : WindowView list) =
        [
            contentsAsDrawingObjects windows.[0].Dimensions windows.[0].Buffer
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