namespace Void.ViewModel

open Void.Core

// Drawing objects are basically considered delta events off the view model
type ViewModelService
    (
        _view : MainView
    ) =
    let mutable _fontMetrics = _view.GetFontMetrics()
    let mutable _convert = Sizing.Convert _fontMetrics
    let mutable _viewModel = ViewModel.defaultViewModel
    let _colorscheme = Colors.defaultColorscheme

    member private x.initializeView() =
        _view.SetViewTitle ViewModel.defaultTitle
        _view.SetBackgroundColor Colors.defaultColorscheme.Background
        x.setFont()
        _view.SetViewSize <| _convert.cellDimensionsToPixels _viewModel.Size
        VMEvent.ViewModelInitialized

    member x.rerenderWholeView() =
        Render.viewModelAsDrawingObjects _convert _viewModel

    member private x.setFont() =
        _view.SetFontBySize ViewModel.defaultFontSize
        _fontMetrics <- _view.GetFontMetrics()
        _convert <- Sizing.Convert _fontMetrics

    member x.handleCommand =
        function
        | Command.InitializeVoid ->
            x.initializeView() :> Message
        | Command.Display _ ->
            notImplemented
        | Command.Redraw ->
            _convert.cellBlockToPixels (ViewModel.wholeArea _viewModel)
            |> VMCommand.Redraw :> Message
        | _ ->
            noMessage

    member x.handleEvent =
        function
        | Event.BufferLoadedIntoWindow buffer ->
            _viewModel <- ViewModel.loadBuffer buffer _viewModel
            let drawings = Render.currentBufferAsDrawingObjects _convert _viewModel
            let area = _convert.cellBlockToPixels <| ViewModel.wholeArea _viewModel (* TODO shouldn't redraw the whole UI *)
            VMEvent.ViewPortionRendered(area, drawings) :> Message
        | Event.CommandEntryCancelled ->
            noMessage
        | Event.CommandMode_CharacterBackspaced ->
            noMessage
        | Event.CommandMode_TextAppended text ->
            noMessage
        | Event.NotificationAdded notification ->
            let drawing = ViewModel.toScreenNotification notification
                          |> Render.notificationAsDrawingObject _convert { Row = CellGrid.lastRow (ViewModel.wholeArea _viewModel); Column = 0 }
            // TODO this is just hacked together for the moment
            let area = _convert.cellBlockToPixels { UpperLeftCell = { Row = CellGrid.lastRow (ViewModel.wholeArea _viewModel); Column = 0 }; Dimensions = { Rows = 1; Columns = _viewModel.Size.Columns }}
            VMEvent.ViewPortionRendered(area, [drawing]) :> Message
        | Event.EditorInitialized editor ->
            _viewModel <- ViewModel.init editor _viewModel 
            let drawings = Render.currentBufferAsDrawingObjects _convert _viewModel
            let area = _convert.cellBlockToPixels <| ViewModel.wholeArea _viewModel
            VMEvent.ViewPortionRendered(area, drawings) :> Message
        | _ -> noMessage
