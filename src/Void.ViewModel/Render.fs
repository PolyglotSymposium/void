namespace Void.ViewModel

open Void.Core

type ScreenLineObject = {
    StartingPoint : PointGrid.Point
    EndingPoint : PointGrid.Point
}

type ScreenTextObject = {
    Text : string
    UpperLeftCorner : PointGrid.Point
    Color : RGBColor
}

type ScreenBlockObject = {
    Area : PointGrid.Block
    Color : RGBColor
}

[<RequireQualifiedAccess>]
type DrawingObject =
    | Line of ScreenLineObject
    | Text of ScreenTextObject
    | Block of ScreenBlockObject

module Render =
    open Void.Core.CellGrid

    let private textLineAsDrawingObject x line =
        DrawingObject.Text {
            Text = line
            UpperLeftCorner = convert.cellToUpperLeftPoint <| below originCell x
            Color = Colors.defaultColorscheme.Foreground
        }

    let textLinesAsDrawingObjects =
        List.mapi textLineAsDrawingObject

    let private commandBarPrompt = 
        DrawingObject.Text {
            Text = ";"
            UpperLeftCorner = convert.cellToUpperLeftPoint { Row = 25; Column = 0 }
            Color = Colors.defaultColorscheme.DimForeground
        }

    let commandBarAsDrawingObjects commandBar width upperLeft =
        DrawingObject.Block {
            Area =
                {
                    UpperLeftCorner = convert.cellToUpperLeftPoint upperLeft
                    Dimensions = { Height = 1; Width = width }
                }
            Color = Colors.defaultColorscheme.Background
        } :: match commandBar with
             | CommandBarView.Hidden -> []
             | CommandBarView.Visible "" -> [commandBarPrompt]
             | CommandBarView.Visible text ->
                 [commandBarPrompt; DrawingObject.Text {
                    Text = text
                    UpperLeftCorner = convert.cellToUpperLeftPoint <| rightOf upperLeft 1
                    Color = Colors.defaultColorscheme.Foreground
                 }]

    let notificationAsDrawingObject upperLeft notification =
        match notification with
        | UserNotificationView.Text text ->
            {
                Text = text
                UpperLeftCorner = convert.cellToUpperLeftPoint upperLeft
                Color = Colors.defaultColorscheme.Foreground
            }
        | UserNotificationView.Error text ->
            {
                Text = text
                UpperLeftCorner = convert.cellToUpperLeftPoint upperLeft
                Color = Colors.defaultColorscheme.Error
            }
        |> DrawingObject.Text

    let notificationsAsDrawingObjects width upperLeft notifications =
        let asDrawingObject =
            notificationAsDrawingObject convert upperLeft
        notifications |> List.map asDrawingObject

    let tabBarAsDrawingObjects tabBar = []

    let bufferAsDrawingObjects windowArea buffer =
        let background = DrawingObject.Block {
            Area = convert.cellBlockToPixels windowArea
            Color = Colors.defaultColorscheme.Background
        }

        let bufferLines = textLinesAsDrawingObjects buffer.Contents

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

    let windowsAsDrawingObjects (windows : WindowView list) =
        bufferAsDrawingObjects windows.[0].Area windows.[0].Buffer

    let viewModelAsDrawingObjects viewModel =
        [
            tabBarAsDrawingObjects viewModel.TabBar
            windowsAsDrawingObjects viewModel.VisibleWindows
            commandBarAsDrawingObjects viewModel.CommandBar viewModel.Size.Columns originCell
            notificationsAsDrawingObjects viewModel.Size.Columns originCell viewModel.Notifications 
        ] |> Seq.concat

    let currentBufferAsDrawingObjects convert viewModel =
        viewModel.VisibleWindows.[0].Buffer
        |> bufferAsDrawingObjects viewModel.VisibleWindows.[0].Area
