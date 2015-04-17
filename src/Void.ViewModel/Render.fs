namespace Void.ViewModel

type ScreenLineObject = {
    StartingPoint : PixelGrid.Point
    EndingPoint : PixelGrid.Point
}

type ScreenTextObject = {
    Text : string
    UpperLeftCorner : PixelGrid.Point
    Color : RGBColor
}

type ScreenBlockObject = {
    Area : PixelGrid.Block
    Color : RGBColor
}

[<RequireQualifiedAccess>]
type DrawingObject =
    | Line of ScreenLineObject
    | Text of ScreenTextObject
    | Block of ScreenBlockObject

module Render =
    open Void.Core.CellGrid

    let private textLineAsDrawingObject (convert : Sizing.Convert) x line =
        DrawingObject.Text {
            Text = line
            UpperLeftCorner = convert.cellToUpperLeftPoint <| below originCell x
            Color = Colors.defaultColorscheme.Foreground
        }

    let textLinesAsDrawingObjects convert lines =
        List.mapi (textLineAsDrawingObject convert) lines

    let private commandBarPrompt (convert : Sizing.Convert) = 
        DrawingObject.Text {
            Text = ";"
            UpperLeftCorner = convert.cellToUpperLeftPoint { Row = 25; Column = 0 }
            Color = Colors.defaultColorscheme.DimForeground
        }

    let commandBarAsDrawingObjects (convert : Sizing.Convert) commandBar width upperLeft =
        DrawingObject.Block {
            Area =
                {
                    UpperLeftCorner = convert.cellToUpperLeftPoint upperLeft
                    Dimensions = { Height = 1; Width = width } // TODO convert to pixels
                }
            Color = Colors.defaultColorscheme.Background
        } :: match commandBar with
             | CommandBarView.Hidden -> []
             | CommandBarView.Visible "" -> [commandBarPrompt convert]
             | CommandBarView.Visible text ->
                 [commandBarPrompt convert; DrawingObject.Text {
                    Text = text
                    UpperLeftCorner = convert.cellToUpperLeftPoint <| rightOf upperLeft 1
                    Color = Colors.defaultColorscheme.Foreground
                 }]

    let outputMessageAsDrawingObject (convert : Sizing.Convert) upperLeft outputMessage =
        match outputMessage with
        | OutputMessageView.Text msg ->
            {
                Text = msg
                UpperLeftCorner = convert.cellToUpperLeftPoint upperLeft
                Color = Colors.defaultColorscheme.Foreground
            }
        | OutputMessageView.Error msg ->
            {
                Text = msg
                UpperLeftCorner = convert.cellToUpperLeftPoint upperLeft
                Color = Colors.defaultColorscheme.Error
            }
        |> DrawingObject.Text

    let outputMessagesAsDrawingObjects convert width upperLeft outputMessages =
        let asDrawingObject =
            outputMessageAsDrawingObject convert upperLeft
        outputMessages |> List.map asDrawingObject

    let tabBarAsDrawingObjects convert tabBar = []

    let bufferAsDrawingObjects (convert : Sizing.Convert) windowArea buffer =
        let background = DrawingObject.Block {
            Area = convert.cellBlockToPixels windowArea
            Color = Colors.defaultColorscheme.Background
        }

        let bufferLines = textLinesAsDrawingObjects convert buffer.Contents

        let rowsNotInBuffer =
            let lineNotInBufferAsDrawingObject i =
                DrawingObject.Text {
                    Text = "~"
                    UpperLeftCorner = convert.cellToUpperLeftPoint { Row = i; Column = 0 }
                    Color = Colors.defaultColorscheme.DimForeground
                }
            let linesWithNoTilde =
                if bufferLines.Length = 0
                then 1
                else bufferLines.Length
            [linesWithNoTilde..windowArea.Dimensions.Rows-1]
            |> List.map lineNotInBufferAsDrawingObject

        List.append (background :: bufferLines) rowsNotInBuffer

    let windowsAsDrawingObjects convert (windows : WindowView list) =
        bufferAsDrawingObjects convert windows.[0].Area windows.[0].Buffer

    let viewModelAsDrawingObjects convert viewModel =
        [
            tabBarAsDrawingObjects convert viewModel.TabBar
            windowsAsDrawingObjects convert viewModel.VisibleWindows
            commandBarAsDrawingObjects convert viewModel.CommandBar viewModel.Size.Columns originCell
            outputMessagesAsDrawingObjects convert viewModel.Size.Columns originCell viewModel.OutputMessages 
        ] |> Seq.concat
