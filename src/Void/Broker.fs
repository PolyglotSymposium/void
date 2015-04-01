namespace Void

open Void.Core
open Void.ViewModel

type Broker
    (
        commandHandlers : (Command -> Command) list,
        eventHandlers : (Event -> Command) list,
        viewCtrl : ViewController
    ) =
    let mutable _commandHandlers = commandHandlers
    let mutable _eventHandlers = eventHandlers

    member x.addCommandHandler commandHandler =
        _commandHandlers <- commandHandler :: _commandHandlers

    // TODO!!! Make this go away
    member x.brokerViewEvent viewEvent =
        match viewEvent with
        | ViewEvent.PaintInitiated draw ->
            viewCtrl.paint draw

    member x.publishCommand command =
        match command with
        | Command.PublishEvent event ->
            for handle in _eventHandlers do
                handle event |> x.publishCommand
        | Command.Noop -> ()
        | _ ->
            for handle in _commandHandlers do
                handle command |> x.publishCommand
