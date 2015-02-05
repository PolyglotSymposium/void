namespace Void.ViewModel

type ScreenTextObject = {
    Text : string
    UpperLeftCorner : CellGrid.Cell
    Color : RGBColor
}

type ScreenBlockObject = {
    Area : CellGrid.Block
    Color : RGBColor
}

[<RequireQualifiedAccess>]
type DrawingObject =
    | Line
    | Text of ScreenTextObject
    | Block of ScreenBlockObject

module Render =
    let private lineAsDrawingObject line =
        DrawingObject.Text {
            Text = line
            UpperLeftCorner = CellGrid.originCell
            Color = Colors.defaultColorscheme.Foreground
        }

    let commandBarPrompt = 
        DrawingObject.Text {
            Text = ";"
            UpperLeftCorner = { Row = 25us; Column = 0us }
            Color = Colors.defaultColorscheme.DimForeground
        }

    let commandBarAsDrawingObjects commandBar width upperLeft =
        DrawingObject.Block {
            Area =
                {
                    UpperLeftCorner = upperLeft
                    Dimensions = { Rows = 1us; Columns = width }
                }
            Color = Colors.defaultColorscheme.Background
        } :: match commandBar with
             | CommandBarView.Hidden -> []
             | CommandBarView.Visible "" -> [commandBarPrompt]
             | CommandBarView.Visible text ->
                 [commandBarPrompt; DrawingObject.Text {
                    Text = text
                    UpperLeftCorner = CellGrid.rightOf upperLeft 1us
                    Color = Colors.defaultColorscheme.Foreground
                 }]

    let private outputMessageAsDrawingObject outputMessage upperLeft =
        match outputMessage with
        | OutputMessage.Text msg ->
            {
                Text = msg
                UpperLeftCorner = upperLeft
                Color = Colors.defaultColorscheme.Foreground
            }
        | OutputMessage.Error msg ->
            {
                Text = msg
                UpperLeftCorner = upperLeft
                Color = Colors.defaultColorscheme.Error
            }
        |> DrawingObject.Text

    let outputMessagesAsDrawingObjects outputMessages width upperLeft =
        let asDrawingObject outputMsg =
            outputMessageAsDrawingObject outputMsg upperLeft
        outputMessages |> Seq.map asDrawingObject

    let linesAsDrawingObjects (viewSize : ViewSize) (lines : string list) =
        List.map lineAsDrawingObject lines
