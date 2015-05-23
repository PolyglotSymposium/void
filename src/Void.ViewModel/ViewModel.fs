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
    let defaultFontSize = 9uy
    let defaultBuffer = { LinesOfText = [] }

    let defaultWindowView containingArea =
        {
            StatusLine = StatusLineView.Focused
            Buffer = defaultBuffer
            Area = lessRowsBelow 1 containingArea
            Cursor = CursorView.Block originCell
        }

    let defaultViewModel =
        {
            Size = Sizing.defaultViewSize
            TabBar = []
            VisibleWindows = [defaultWindowView Sizing.defaultViewArea]
            CommandBar = CommandBar.hidden
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
        Editor.readLines buffer 1
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

    let areaOfCommandBarOrNotifications viewModel =
        // TODO this is just hacked together for the moment
        {
            UpperLeftCell = { Row = CellGrid.lastRow (wholeArea viewModel); Column = 0 }
            Dimensions = { Rows = 1; Columns = viewModel.Size.Columns }
        }

    let appendTextInCommandBar viewModel textToAppend =
        let area = areaOfCommandBarOrNotifications viewModel
        let commandBar, msg = CommandBar.appendText viewModel.CommandBar area textToAppend
        ({ viewModel with CommandBar = commandBar }, msg)

    let characterBackspacedInCommandBar viewModel =
        let area = areaOfCommandBarOrNotifications viewModel
        let commandBar, msg = CommandBar.characterBackspaced area viewModel.CommandBar
        ({ viewModel with CommandBar = commandBar }, msg)

    let hideCommandBar viewModel =
        let area = areaOfCommandBarOrNotifications viewModel
        let commandBar, msg = CommandBar.hide area
        ({ viewModel with CommandBar = commandBar }, msg)

    let showCommandBar viewModel =
        let area = areaOfCommandBarOrNotifications viewModel
        let commandBar, msg = CommandBar.show area
        ({ viewModel with CommandBar = commandBar }, msg)

    let init editorState viewModel =
        loadBuffer (Editor.currentBuffer editorState) viewModel
