namespace Void.ViewModel

open Void.Core
open CellGrid

type ViewController
    (
        _view : MainView
    ) =
    let mutable _fontMetrics = Sizing.defaultFontMetrics
    let mutable _viewSize = Sizing.defaultViewSize
    let _colorscheme = Colors.defaultColorscheme

    member x.initializeView() =
        _view.SetViewTitle ViewModel.defaultTitle
        _view.SetBackgroundColor Colors.defaultColorscheme.Background
        _view.SetFontBySize ViewModel.defaultFontSize
        _view.SetViewSize <| Sizing.cellDimensionsToPixels _fontMetrics _viewSize
        _view.SubscribeToDraw(fun artist -> x.draw artist)

    // TODO refactor to architecture
    member private x.draw artist =
        let mutable i = 0
        for line in ["a"; "b"; "c"] do
            let offset = _fontMetrics.LineHeight * i
            i <- i + 1
            x.textOnRow artist line offset

    member private x.textOnRow artist text row =
        artist.RenderText { Text = text; UpperLeftCorner = { X = 0; Y = row }; Color = _colorscheme.Foreground }

    member private x.render (artist : Artist) drawingObject =
        match drawingObject with
        | DrawingObject.Line -> () // TODO
        | DrawingObject.Block block -> artist.RenderBlock block
        | DrawingObject.Text text -> artist.RenderText text

    member x.handleCommand command =
        match command with
        | Command.Quit -> _view.Close()
        | Command.Redraw -> _view.Redraw()
        | _ -> ()
        Command.Noop

    member x.handleEvent event =
        match event with
        | Event.MessageAdded msg ->
            Command.Noop // TODO
        | Event.BufferLoadedIntoWindow buffer ->
            // TODO Editor.readLines buffer 1
            Command.Redraw
        | _ -> Command.Noop

type MainController
    (
        _view : MainView
    ) =
    let _normalCtrl = NormalModeController()
    let _coreCtrl = CoreController()
    let _viewCtrl = ViewController(_view)
    let _eventHandlers = [_coreCtrl.handleEvent; _viewCtrl.handleEvent]

    member x.initializeVoid() =
        _viewCtrl.initializeView()
        // In general, ViewController should operate on the view, not MainController.
        // However, the lines below are input, not painting/drawing/displaying...
        _view.SubscribeToKeyUp (fun keyPress ->
            _normalCtrl.handle keyPress |> x.handleCommand
        )
        x.handleCommand Command.ViewTestBuffer

    member private x.handleCommand command =
        let notImplemented() =
            Event.ErrorOccurred Error.NotImplemented
            |> Command.PublishEvent
            |> x.handleCommand

        match command with
        | Command.ChangeToMode _
        | Command.Edit
        | Command.FormatCurrentLine ->
            notImplemented()
        | Command.PublishEvent event ->
            for handle in _eventHandlers do
                handle event |> x.handleCommand
        | Command.Quit
        | Command.Redraw ->
            _viewCtrl.handleCommand command |> x.handleCommand
        | Command.ViewTestBuffer -> 
            _coreCtrl.handleCommand command |> x.handleCommand
        | Command.Noop -> ()
