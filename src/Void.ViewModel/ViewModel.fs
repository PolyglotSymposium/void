namespace Void.ViewModel

open Void.Core

module Sizing =
    open Void.Core.CellGrid
    let defaultViewSize = { Rows = 26; Columns = 80 }
    let defaultViewArea = { UpperLeftCell = originCell; Dimensions = defaultViewSize }

module ViewModel =
    open Void.Util
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
            CommandBar = CommandBarView.Hidden
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

    let appendTextInCommandBar viewModel textToAppend =
        match viewModel.CommandBar with
        | CommandBarView.Hidden ->
            { viewModel with CommandBar = CommandBarView.Visible textToAppend }
        | CommandBarView.Visible text ->
            { viewModel with CommandBar = CommandBarView.Visible (text + textToAppend) }

    let areaOfCommandBarOrNotifications viewModel =
        // TODO this is just hacked together for the moment
        {
            UpperLeftCell = { Row = CellGrid.lastRow (wholeArea viewModel); Column = 0 }
            Dimensions = { Rows = 1; Columns = viewModel.Size.Columns }
        }

    let characterBackspacedInCommandBar viewModel =
        match viewModel.CommandBar with
        | CommandBarView.Hidden ->
            (viewModel, noMessage)
        | CommandBarView.Visible text ->
            let area = areaOfCommandBarOrNotifications viewModel
            let vm = { viewModel with CommandBar = CommandBarView.Visible <| text.Remove(text.Length - 1) }
            let areaInPoints = GridConvert.boxAroundOneCell <| CellGrid.rightOf area.UpperLeftCell text.Length
            let drawing = DrawingObject.Block {
                Area = areaInPoints
                Color = Colors.defaultColorscheme.Background
            }
            (vm, VMEvent.ViewPortionRendered(areaInPoints, [drawing]) :> Message)

    let hideCommandBar viewModel =
        let area = areaOfCommandBarOrNotifications viewModel
        let vm = { viewModel with CommandBar = CommandBarView.Hidden }
        let drawings = Render.commandBarAsDrawingObjects vm.CommandBar area.Dimensions.Columns area.UpperLeftCell
        let areaInPoints = GridConvert.boxAround area
        (vm, VMEvent.ViewPortionRendered(areaInPoints, drawings) :> Message)

    let showCommandBar viewModel =
        let area = areaOfCommandBarOrNotifications viewModel
        let vm = { viewModel with CommandBar = CommandBarView.Visible "" }
        let drawings = Render.commandBarAsDrawingObjects vm.CommandBar area.Dimensions.Columns area.UpperLeftCell
        let areaInPoints = GridConvert.boxAround area
        (vm, VMEvent.ViewPortionRendered(areaInPoints, drawings) :> Message)

    let init editorState viewModel =
        loadBuffer (Editor.currentBuffer editorState) viewModel
