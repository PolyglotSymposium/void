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

    let contentsAsDrawingObjects windowArea (buffer : string list) =
        let background = DrawingObject.Block {
            Area = GridConvert.boxAround windowArea
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
            [linesWithNoTilde..windowArea.Dimensions.Rows-1]
            |> List.map lineNotInBufferAsDrawingObject

        List.append (background :: bufferLines) rowsNotInBuffer

    let private renderWindow area contents =
        let drawings = contentsAsDrawingObjects area contents
        VMEvent.ViewPortionRendered(GridConvert.boxAround area, drawings) :> Message

    let asDrawingObjects (windows : WindowView list) =
        [
            contentsAsDrawingObjects windows.[0].Area windows.[0].Buffer
        ] |> Seq.concat

    let handleWindowEvent area event =
        match event with
        | Window.Event.ContentsUpdated contents ->
            renderWindow !area contents
        | Window.Event.Initialized window ->
            renderWindow !area window.Buffer

    let handleWindowCommand area (Window.Command.RedrawWindow window) =
        renderWindow !area window.Buffer

    module Service =
        let subscribe (bus : Bus) =
            let area = ref zeroBlock
            handleWindowEvent area |> bus.subscribe
            handleWindowCommand area |> bus.subscribe