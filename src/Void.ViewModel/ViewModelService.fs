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

    member x.handleVMEvent event =
        let commandBarOrigin = ViewModel.upperLeftCellOfCommandBar _viewModel
        let renderCommandBar commandBar =
            Render.commandBarAsDrawingObjects commandBar commandBarOrigin
            |> VMEvent.ViewPortionRendered :> Message
        match event with
        | VMEvent.CommandBar_CharacterBackspacedFromLine cell ->
            // TODO refactor -- move into Render module
            let offsetCell = CellGrid.vectorAdd commandBarOrigin cell 
            let area = GridConvert.boxAroundOneCell offsetCell
            let drawing = DrawingObject.Block {
                Area = area
                Color = Colors.defaultColorscheme.Background
            }
            VMEvent.ViewPortionRendered (area, Seq.singleton drawing) :> Message
        | VMEvent.CommandBar_Displayed commandBar ->
            renderCommandBar commandBar
        | VMEvent.CommandBar_Hidden commandBar ->
            renderCommandBar commandBar
        | VMEvent.CommandBar_TextAppendedToLine textSegment ->
            // TODO refactor -- move into Render module
            let offsetCell = CellGrid.vectorAdd commandBarOrigin textSegment.LeftMostCell 
            let area = GridConvert.boxAroundOneCell offsetCell // TODO but what if it's not one cell?
            let drawing = DrawingObject.Text {
                Text = textSegment.Text
                UpperLeftCorner = GridConvert.upperLeftCornerOf offsetCell
                Color = Colors.defaultColorscheme.Foreground
            }
            VMEvent.ViewPortionRendered (area, Seq.singleton drawing) :> Message
        | VMEvent.CommandBar_TextChanged commandBar ->
            renderCommandBar commandBar
        | VMEvent.CommandBar_TextReflowed commandBar ->
            renderCommandBar commandBar
        | _ -> noMessage

    member x.handleEvent =
        function // TODO clearly the code below needs to be refactored
        | Event.BufferLoadedIntoWindow buffer ->
            _viewModel <- ViewModel.loadBuffer buffer _viewModel
            let drawings = Render.currentBufferAsDrawingObjects _viewModel
            let area = GridConvert.boxAround (ViewModel.wholeArea _viewModel) (* TODO shouldn't redraw the whole UI *)
            VMEvent.ViewPortionRendered(area, drawings) :> Message
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
