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

    let private commandBarPrompt upperLeftCell prompt = 
        DrawingObject.Text {
            Text = match prompt with
                   | CommandBarPrompt.ClassicVim -> ":"
                   | CommandBarPrompt.VoidDefault -> ";"
            UpperLeftCorner = GridConvert.upperLeftCornerOf upperLeftCell
            Color = Colors.defaultColorscheme.DimForeground
        }

    let private commandBarLines upperLeftCell lines =
        let startingCellForLineNumber i =
           if i = 0
           then rightOf upperLeftCell 1
           else below upperLeftCell i
           |> GridConvert.upperLeftCornerOf
        let renderLine i text =
            DrawingObject.Text {
               Text = text
               UpperLeftCorner = startingCellForLineNumber i
               Color = Colors.defaultColorscheme.Foreground
            }
        lines
        |> List.filter (fun line -> line <> "")
        |> List.mapi renderLine

    let private commandBarInArea commandBar area upperLeftCell =
        DrawingObject.Block {
            Area = area
            Color = Colors.defaultColorscheme.Background
        } :: match commandBar with
             | { Prompt = Hidden; WrappedLines = _ } -> []
             | { Prompt = Visible prompt; WrappedLines = lines } ->
                 commandBarPrompt upperLeftCell prompt :: commandBarLines upperLeftCell lines
        |> Seq.ofList

    let commandBarAsDrawingObjects commandBar upperLeftCell =
        let height =
            if commandBar = CommandBar.hidden
            then 1
            else commandBar.WrappedLines.Length
        let area : PointGrid.Block = {
            UpperLeftCorner = GridConvert.upperLeftCornerOf upperLeftCell
            Dimensions = { Height = height; Width = commandBar.Width }
        }
        (area, commandBarInArea commandBar area upperLeftCell)

    let notificationAsDrawingObject upperLeft notification =
        match notification with
        | UserNotificationView.Text text ->
            {
                Text = text
                UpperLeftCorner = GridConvert.upperLeftCornerOf upperLeft
                Color = Colors.defaultColorscheme.Foreground
            }
        | UserNotificationView.Error text ->
            {
                Text = text
                UpperLeftCorner = GridConvert.upperLeftCornerOf upperLeft
                Color = Colors.defaultColorscheme.Error
            }
        |> DrawingObject.Text

    let notificationsAsDrawingObjects width upperLeft notifications =
        let asDrawingObject =
            notificationAsDrawingObject upperLeft
        notifications |> List.map asDrawingObject

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
            notificationsAsDrawingObjects viewModel.Size.Columns originCell viewModel.Notifications 
        ] |> Seq.concat

    let currentBufferAsDrawingObjects viewModel =
        viewModel.VisibleWindows.[0].Buffer
        |> bufferAsDrawingObjects viewModel.VisibleWindows.[0].Area

