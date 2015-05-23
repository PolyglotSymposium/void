namespace Void.ViewModel

open Void.Core

// Drawing objects are basically considered delta events off the view model
type ViewModelService
    (
        _view : MainView
    ) =
    let mutable _viewModel = ViewModel.defaultViewModel
    let _colorscheme = Colors.defaultColorscheme

    member private x.initializeView() =
        _view.SetViewTitle ViewModel.defaultTitle
        _view.SetBackgroundColor Colors.defaultColorscheme.Background
        x.setFont()
        _view.SetViewSize <| GridConvert.dimensionsInPoints _viewModel.Size
        VMEvent.ViewModelInitialized

    member x.rerenderWholeView() =
        Render.viewModelAsDrawingObjects _viewModel

    member private x.setFont() =
        _view.SetFontBySize ViewModel.defaultFontSize

    member x.handleCommand =
        function
        | Command.InitializeVoid ->
            x.initializeView() :> Message
        | Command.Display _ ->
            notImplemented
        | Command.Redraw ->
            GridConvert.boxAround (ViewModel.wholeArea _viewModel)
            |> VMCommand.Redraw :> Message
        | _ ->
            noMessage

    member x.handleEvent =
        function // TODO clearly the code below needs to be refactored
        | Event.BufferLoadedIntoWindow buffer ->
            _viewModel <- ViewModel.loadBuffer buffer _viewModel
            let drawings = Render.currentBufferAsDrawingObjects _viewModel
            let area = GridConvert.boxAround (ViewModel.wholeArea _viewModel) (* TODO shouldn't redraw the whole UI *)
            VMEvent.ViewPortionRendered(area, drawings) :> Message
        | Event.ModeChanged { From = _; To = Mode.Command } ->
            let viewModel, event = ViewModel.showCommandBar _viewModel
            _viewModel <- viewModel
            event
        | Event.CommandEntryCancelled ->
            let viewModel, event = ViewModel.hideCommandBar _viewModel
            _viewModel <- viewModel
            event
        | Event.CommandMode_CharacterBackspaced ->
            let viewModel, event = ViewModel.characterBackspacedInCommandBar _viewModel
            _viewModel <- viewModel
            event
        | Event.CommandMode_TextAppended text ->
            let area = ViewModel.areaOfCommandBarOrNotifications _viewModel
            _viewModel <- ViewModel.appendTextInCommandBar _viewModel text
            let drawings = Render.commandBarAsDrawingObjects _viewModel.CommandBar area.Dimensions.Columns area.UpperLeftCell
            let areaInPoints = GridConvert.boxAround area // TODO we are refreshing too much
            VMEvent.ViewPortionRendered(areaInPoints, drawings) :> Message
        | Event.NotificationAdded notification ->
            let area = ViewModel.areaOfCommandBarOrNotifications _viewModel
            let drawing = ViewModel.toScreenNotification notification
                          |> Render.notificationAsDrawingObject area.UpperLeftCell
            let areaInPoints = GridConvert.boxAround area
            VMEvent.ViewPortionRendered(areaInPoints, [drawing]) :> Message
        | Event.EditorInitialized editor ->
            _viewModel <- ViewModel.init editor _viewModel 
            let drawings = Render.currentBufferAsDrawingObjects _viewModel
            let area = GridConvert.boxAround <| ViewModel.wholeArea _viewModel
            VMEvent.ViewPortionRendered(area, drawings) :> Message
        | _ -> noMessage
