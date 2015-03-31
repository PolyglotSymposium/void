namespace Void

open Void.Core
open Void.ViewModel

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
