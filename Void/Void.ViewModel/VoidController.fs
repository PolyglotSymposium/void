﻿namespace Void.ViewModel

open Void.Core
open CellGrid

type ViewController
    (
        _view : MainView
    ) =
    let mutable _fontMetrics = _view.GetFontMetrics()
    let mutable _convert = Sizing.Convert _fontMetrics
    let _viewArea = { UpperLeftCell = originCell; Dimensions = Sizing.defaultViewSize }
    let _colorscheme = Colors.defaultColorscheme
    let mutable _bufferedDrawings = []

    // Subscribe to some init event on the view instead of exposing this as a
    // public method?
    member private x.initializeView() =
        _view.SetViewTitle ViewModel.defaultTitle
        _view.SetBackgroundColor Colors.defaultColorscheme.Background
        x.setFont()
        _view.SetViewSize <| _convert.cellDimensionsToPixels _viewArea.Dimensions
        _view.SubscribeToDraw(fun draw -> for drawing in _bufferedDrawings do draw.Invoke drawing)
        Command.PublishEvent Event.ViewInitialized

    member private x.setFont() =
        _view.SetFontBySize ViewModel.defaultFontSize
        _fontMetrics <- _view.GetFontMetrics()
        _convert <- Sizing.Convert _fontMetrics

    member x.handleCommand command =
        match command with
        | Command.Quit -> _view.Close()
        | Command.Redraw -> _view.TriggerDraw()
        | _ -> ()
        Command.Noop

    member x.handleEvent event =
        match event with
        | Event.MessageAdded msg ->
            Command.Noop // TODO
        | Event.BufferLoadedIntoWindow buffer ->
            _bufferedDrawings <- ViewModel.toScreenBuffer _viewArea.Dimensions buffer
                                 |> Render.bufferAsDrawingObjects _convert _viewArea
            Command.Redraw
        | Event.CoreInitialized ->x.initializeView()
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
        // In general, ViewController should operate on the view, not MainController.
        // However, the lines below are input, not painting/drawing/displaying...
        _view.SubscribeToKeyUp (fun keyPress ->
            _normalCtrl.handle keyPress |> x.handleCommand
        )
        x.handleCommand <| Command.PublishEvent Event.CoreInitialized
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
