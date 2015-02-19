namespace Void.ViewModel

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
    | Line
    | Text of ScreenTextObject
    | Block of ScreenBlockObject

module Render =
    open CellGrid

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

    let private outputMessageAsDrawingObject (convert : Sizing.Convert) outputMessage upperLeft =
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

    let outputMessagesAsDrawingObjects convert outputMessages width upperLeft =
        let asDrawingObject outputMsg =
            outputMessageAsDrawingObject convert outputMsg upperLeft
        outputMessages |> List.map asDrawingObject

    let tabBarAsDrawingObjects convert tabBar = []


    let bufferAsDrawingObjects (convert : Sizing.Convert) windowArea buffer =
        let lineNotInBufferAsDrawingObject i =
            DrawingObject.Text {
                Text = "~"
                UpperLeftCorner = convert.cellToUpperLeftPoint { Row = i; Column = 0 }
                Color = Colors.defaultColorscheme.DimForeground
            }

        let background = DrawingObject.Block {
            Area = convert.cellBlockToPixels windowArea
            Color = Colors.defaultColorscheme.Background
        }

        let foreground = textLinesAsDrawingObjects convert buffer.Contents

        let rowsNotInBuffer =
            [1..windowArea.Dimensions.Rows-1] |> List.map lineNotInBufferAsDrawingObject

        List.append (background :: foreground) rowsNotInBuffer

    let windowsAsDrawingObjects convert window = []

    let viewModelAsDrawingObjects convert viewModel =
        [
            tabBarAsDrawingObjects convert viewModel.TabBar
            windowsAsDrawingObjects convert viewModel.VisibleWindows
            commandBarAsDrawingObjects convert viewModel.CommandBar viewModel.Size.Columns originCell
            outputMessagesAsDrawingObjects convert viewModel.OutputMessages viewModel.Size.Columns originCell
        ] |> Seq.concat
