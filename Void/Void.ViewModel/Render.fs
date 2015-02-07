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

    let private textLineAsDrawingObject cellToPoint x line =
        DrawingObject.Text {
            Text = line
            UpperLeftCorner = cellToPoint <| below originCell (uint16 x)
            Color = Colors.defaultColorscheme.Foreground
        }

    let textLinesAsDrawingObjects cellToPoint lines =
        List.mapi (textLineAsDrawingObject cellToPoint) lines

    let private commandBarPrompt cellToPoint = 
        DrawingObject.Text {
            Text = ";"
            UpperLeftCorner = cellToPoint { Row = 25us; Column = 0us }
            Color = Colors.defaultColorscheme.DimForeground
        }

    let commandBarAsDrawingObjects cellToPoint commandBar width upperLeft =
        DrawingObject.Block {
            Area =
                {
                    UpperLeftCorner = cellToPoint upperLeft
                    Dimensions = { Height = 1us; Width = width } // TODO convert to pixels
                }
            Color = Colors.defaultColorscheme.Background
        } :: match commandBar with
             | CommandBarView.Hidden -> []
             | CommandBarView.Visible "" -> [commandBarPrompt cellToPoint]
             | CommandBarView.Visible text ->
                 [commandBarPrompt cellToPoint; DrawingObject.Text {
                    Text = text
                    UpperLeftCorner = cellToPoint <| rightOf upperLeft 1us
                    Color = Colors.defaultColorscheme.Foreground
                 }]

    let private outputMessageAsDrawingObject cellToPoint outputMessage upperLeft =
        match outputMessage with
        | OutputMessageView.Text msg ->
            {
                Text = msg
                UpperLeftCorner = cellToPoint upperLeft
                Color = Colors.defaultColorscheme.Foreground
            }
        | OutputMessageView.Error msg ->
            {
                Text = msg
                UpperLeftCorner = cellToPoint upperLeft
                Color = Colors.defaultColorscheme.Error
            }
        |> DrawingObject.Text

    let outputMessagesAsDrawingObjects cellToPoint outputMessages width upperLeft =
        let asDrawingObject outputMsg =
            outputMessageAsDrawingObject cellToPoint outputMsg upperLeft
        outputMessages |> List.map asDrawingObject

    let tabBarAsDrawingObjects x y z = []
    let windowsAsDrawingObjects x y z = []

    let viewModelAsDrawingObjects cellToPoint dimensionInPixels viewModel =
        [
            tabBarAsDrawingObjects cellToPoint dimensionInPixels viewModel.TabBar
            windowsAsDrawingObjects cellToPoint dimensionInPixels viewModel.VisibleWindows
            commandBarAsDrawingObjects cellToPoint viewModel.CommandBar viewModel.Size.Columns originCell
            outputMessagesAsDrawingObjects cellToPoint viewModel.OutputMessages viewModel.Size.Columns originCell
        ] |> Seq.concat