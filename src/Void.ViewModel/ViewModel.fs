namespace Void.ViewModel

module Sizing =
    open Void.Core.CellGrid
    let defaultViewSize = { Rows = 26; Columns = 80 }
    let defaultViewArea = { UpperLeftCell = originCell; Dimensions = defaultViewSize }

module ViewModel =
    open Void.Util
    open Void.Core
    open Void.Core.CellGrid

    let defaultTitle = "Void - A text editor in the spirit of Vim"
    let defaultFontSize = 9
    let defaultBuffer = { LinesOfText = [] }

    let defaultWindowView containingArea =
        {
            StatusLine = StatusLineView.Focused
            Buffer = defaultBuffer
            Area = lessRowsBelow 1 containingArea
            Cursor = Visible <| CursorView.Block originCell
        }

    let defaultViewModel =
        {
            Size = Sizing.defaultViewSize
            Title = defaultTitle
            BackgroundColor = Colors.defaultColorscheme.Background
            FontSize = defaultFontSize
            TabBar = []
            VisibleWindows = [defaultWindowView Sizing.defaultViewArea]
            Notifications = []
        }

    let bufferFrom (windowSize : Dimensions) lines =
        let truncateToWindowWidth = StringUtil.noLongerThan windowSize.Columns
        {
            LinesOfText = lines
            |> SeqUtil.notMoreThan windowSize.Rows
            |> Seq.map truncateToWindowWidth
            |> Seq.toList
        }

    let toScreenBuffer windowSize buffer =
        Buffer.readLines buffer 1
        |> bufferFrom windowSize

    let private loadBufferIntoWindow buffer window =
        { window with Buffer = toScreenBuffer window.Area.Dimensions buffer }

    let loadBuffer buffer view =
        { view with VisibleWindows = [loadBufferIntoWindow buffer view.VisibleWindows.[0]] }

    let wholeArea view =
        {
            UpperLeftCell = originCell
            Dimensions = view.Size
        }

    let toScreenNotification =
        function
        | UserNotification.Output notificationText -> UserNotificationView.Text notificationText
        | UserNotification.Error error -> UserNotificationView.Error <| Errors.textOf error

    let addNotification viewModel notification =
        { viewModel with Notifications = notification :: viewModel.Notifications }

    let upperLeftCellOfCommandBar viewModel =
        // TODO this is just hacked together for the moment
        { Row = CellGrid.lastRow (wholeArea viewModel); Column = 0 }

    let areaOfCommandBarOrNotifications viewModel =
        // TODO this is just hacked together for the moment
        {
            UpperLeftCell = upperLeftCellOfCommandBar viewModel
            Dimensions = { Rows = 1; Columns = viewModel.Size.Columns }
        }
