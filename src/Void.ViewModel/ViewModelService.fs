namespace Void.ViewModel

open Void.Core

type ViewModelService() =
    let mutable _viewModel = ViewModel.defaultViewModel

    member x.handleCommand =
        function
        | Command.InitializeVoid ->
            VMEvent.ViewModelInitialized _viewModel :> Message
        | Command.Display _ ->
            notImplemented
        | Command.Redraw ->
            (GridConvert.boxAround (ViewModel.wholeArea _viewModel), Render.viewModelAsDrawingObjects _viewModel)
            |> VMEvent.ViewPortionRendered :> Message
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
            let viewModel, msg = ViewModel.showCommandBar _viewModel
            _viewModel <- viewModel
            msg
        | Event.CommandEntryCancelled ->
            let viewModel, msg = ViewModel.hideCommandBar _viewModel
            _viewModel <- viewModel
            msg
        | Event.CommandMode_CharacterBackspaced ->
            let viewModel, msg = ViewModel.characterBackspacedInCommandBar _viewModel
            _viewModel <- viewModel
            msg
        | Event.CommandMode_TextAppended text ->
            let viewModel, msg = ViewModel.appendTextInCommandBar _viewModel text
            _viewModel <- viewModel
            msg
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
