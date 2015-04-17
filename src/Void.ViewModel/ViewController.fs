namespace Void.ViewModel

open Void.Core
open Void.Core.CellGrid
open System

type ViewController
    (
        _view : MainView
    ) =
    let mutable _fontMetrics = _view.GetFontMetrics()
    let mutable _convert = Sizing.Convert _fontMetrics
    let mutable _viewModel = ViewModel.defaultViewModel
    let _colorscheme = Colors.defaultColorscheme
    let mutable _bufferedDrawings = Seq.empty

    member private x.initializeView() =
        _view.SubscribeToPaint x.paint
        _view.SetViewTitle ViewModel.defaultTitle
        _view.SetBackgroundColor Colors.defaultColorscheme.Background
        x.setFont()
        _view.SetViewSize <| _convert.cellDimensionsToPixels _viewModel.Size
        Command.PublishEvent Event.ViewInitialized

    member private x.paint (draw : Action<DrawingObject>) =
        let drawAll drawings = for drawing in drawings do draw.Invoke drawing
        if Seq.isEmpty _bufferedDrawings
        then drawAll <| Render.viewModelAsDrawingObjects _convert _viewModel
        else drawAll _bufferedDrawings
        _bufferedDrawings <- []

    member private x.setFont() =
        _view.SetFontBySize ViewModel.defaultFontSize
        _fontMetrics <- _view.GetFontMetrics()
        _convert <- Sizing.Convert _fontMetrics

    member private x.bufferDrawings drawings =
        _bufferedDrawings <- Seq.append _bufferedDrawings drawings

    member private x.bufferDrawing drawing =
        x.bufferDrawings [drawing] // TODO is there a better way to do this?

    member x.handleCommand command =
        match command with
        | Command.InitializeVoid -> x.initializeView()
        | Command.Display _ ->
            notImplemented
        | Command.Redraw ->
            _convert.cellBlockToPixels (ViewModel.wholeArea _viewModel) |> _view.TriggerDraw
            Command.Noop
        | _ ->
            Command.Noop

    member x.handleEvent event =
        match event with
        | Event.BufferLoadedIntoWindow buffer ->
            _viewModel <- ViewModel.loadBuffer buffer _viewModel
            Render.currentBufferAsDrawingObjects _convert _viewModel
            |> x.bufferDrawings
            _convert.cellBlockToPixels (ViewModel.wholeArea _viewModel) |> _view.TriggerDraw // TODO shouldn't redraw the whole UI
        | Event.MessageAdded msg ->
            ViewModel.toScreenMessage msg
            |> Render.outputMessageAsDrawingObject _convert { Row = lastRow (ViewModel.wholeArea _viewModel); Column = 0 }
            |> x.bufferDrawing
            // TODO this is just hacked together for the moment
            _convert.cellBlockToPixels { UpperLeftCell = { Row = lastRow (ViewModel.wholeArea _viewModel); Column = 0 }; Dimensions = { Rows = 1; Columns = _viewModel.Size.Columns }} |> _view.TriggerDraw
        | Event.LastWindowClosed ->
            _view.Close()
        | Event.EditorInitialized editor ->
            _viewModel <- ViewModel.loadBuffer (Editor.currentBuffer editor) _viewModel 
            // TODO duplication of BufferLoadedIntoWindow below
            Render.currentBufferAsDrawingObjects _convert _viewModel
            |> x.bufferDrawings
            _convert.cellBlockToPixels (ViewModel.wholeArea _viewModel) |> _view.TriggerDraw // TODO shouldn't redraw the whole UI
        | _ -> ()
        Command.Noop
