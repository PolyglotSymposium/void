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
    let mutable _windowView = ViewModel.defaultFocusedWindowView
    let _colorscheme = Colors.defaultColorscheme
    let mutable _bufferedDrawings = Seq.empty

    // Subscribe to some init event on the view instead of exposing this as a
    // public method?
    member private x.initializeView() =
        _view.SetViewTitle ViewModel.defaultTitle
        _view.SetBackgroundColor Colors.defaultColorscheme.Background
        x.setFont()
        _view.SetViewSize <| _convert.cellDimensionsToPixels _windowView.Area.Dimensions
        Command.PublishEvent Event.ViewInitialized

    member x.paint (draw : Action<DrawingObject>) =
        for drawing in _bufferedDrawings do draw.Invoke drawing
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
            _convert.cellBlockToPixels _windowView.Area |> _view.TriggerDraw
            Command.Noop
        | _ ->
            Command.Noop

    member x.handleEvent event =
        match event with
        | Event.BufferLoadedIntoWindow buffer ->
            _windowView <- ViewModel.loadBufferInto buffer _windowView
            _windowView.Buffer
            |> Render.bufferAsDrawingObjects _convert _windowView.Area
            |> x.bufferDrawings
            _convert.cellBlockToPixels _windowView.Area |> _view.TriggerDraw // TODO shouldn't redraw the whole UI
        | Event.MessageAdded msg ->
            ViewModel.toScreenMessage msg
            |> Render.outputMessageAsDrawingObject _convert { Row = lastRow _windowView.Area; Column = 0 }
            |> x.bufferDrawing
            // TODO this is just hacked together for the moment
            _convert.cellBlockToPixels { UpperLeftCell = { Row = lastRow _windowView.Area; Column = 0 }; Dimensions = { Rows = 1; Columns = _windowView.Area.Dimensions.Columns }} |> _view.TriggerDraw
        | Event.LastWindowClosed ->
            _view.Close()
        | Event.EditorInitialized editor ->
            _windowView <- ViewModel.loadBufferInto (Editor.currentBuffer editor) _windowView 
            // TODO duplication of BufferLoadedIntoWindow below
            _windowView.Buffer
            |> Render.bufferAsDrawingObjects _convert _windowView.Area
            |> x.bufferDrawings
            _convert.cellBlockToPixels _windowView.Area |> _view.TriggerDraw // TODO shouldn't redraw the whole UI
        | _ -> ()
        Command.Noop

type Broker
    (
        _commandHandlers : (Command -> Command) list,
        _eventHandlers : (Event -> Command) list,
        _viewCtrl : ViewController,
        _normalCtrl : NormalModeController
    ) =

    member x.brokerViewEvent viewEvent =
        match viewEvent with
        | ViewEvent.PaintInitiated draw ->
            _viewCtrl.paint draw
        | ViewEvent.KeyPressed keyPress ->
            _normalCtrl.handleKeyPress keyPress |> x.brokerCommand
        | ViewEvent.TextEntered text ->
            () // TODO implement input and command modes, etc

    member x.brokerCommand command =
        match command with
        | Command.PublishEvent event ->
            for handle in _eventHandlers do
                handle event |> x.brokerCommand
        | Command.Noop -> ()
        | _ ->
            for handle in _commandHandlers do
                handle command |> x.brokerCommand

module Init =
    let initializeVoid view =
        let modeCtrl = ModeController()
        let messageCtrl = MessageController()
        let normalCtrl = NormalModeController()
        let editorCtrl = EditorController()
        let viewCtrl = ViewController view
        let commandHandlers = [
            messageCtrl.handleCommand
            modeCtrl.handleCommand
            viewCtrl.handleCommand
            editorCtrl.handleCommand
        ]
        let eventHandlers = [
            messageCtrl.handleEvent
            viewCtrl.handleEvent
        ]
        let broker = Broker(commandHandlers, eventHandlers, viewCtrl, normalCtrl)
        broker.brokerCommand Command.InitializeVoid
        //broker.brokerCommand Command.ViewTestBuffer // TODO for testing and debugging only
        broker.brokerViewEvent