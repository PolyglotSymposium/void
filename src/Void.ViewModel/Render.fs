namespace Void.ViewModel

open Void.Core
module Render =
    open Void.Core.CellGrid

    let private textLineAsDrawingObject x line =
        DrawingObject.Text {
            Text = line
            UpperLeftCorner = GridConvert.upperLeftCornerOf <| below originCell x
            Color = Colors.defaultColorscheme.Foreground
        }

    let textLinesAsDrawingObjects =
        List.mapi textLineAsDrawingObject

    let tabBarAsDrawingObjects tabBar = []

    let bufferAsDrawingObjects windowArea (buffer : BufferView) =
        let background = DrawingObject.Block {
            Area = GridConvert.boxAround windowArea
            Color = Colors.defaultColorscheme.Background
        }

        let bufferLines = textLinesAsDrawingObjects buffer.LinesOfText

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

    let windowsAsDrawingObjects (windows : WindowView list) =
        bufferAsDrawingObjects windows.[0].Area windows.[0].Buffer

    let viewModelAsDrawingObjects viewModel =
        [
            tabBarAsDrawingObjects viewModel.TabBar
            windowsAsDrawingObjects viewModel.VisibleWindows
        ] |> Seq.concat

    let currentBufferAsDrawingObjects viewModel =
        viewModel.VisibleWindows.[0].Buffer
        |> bufferAsDrawingObjects viewModel.VisibleWindows.[0].Area

