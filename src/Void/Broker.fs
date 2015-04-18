namespace Void

open Void.Core
open Void.ViewModel

type Broker
    (
        commandHandlers : (Command -> Message) list,
        eventHandlers : (Event -> Message) list
    ) =
    let mutable _commandHandlers = commandHandlers
    let mutable _eventHandlers = eventHandlers

    member x.addCommandHandler commandHandler =
        _commandHandlers <- commandHandler :: _commandHandlers

    member x.addEventHandler eventHandler =
        _eventHandlers <- eventHandler :: _eventHandlers

    member x.publish (message : Message) =
        match message with
        | :? Event as event ->
            for handle in _eventHandlers do
                handle event |> x.publish
        | :? Command as command ->
            for handle in _commandHandlers do
                handle command |> x.publish
        | noMessage -> ()
