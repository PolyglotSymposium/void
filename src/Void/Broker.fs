namespace Void

open Void.Core
open Void.ViewModel

type Broker
    (
        commandHandlers : (Command -> Message) list,
        eventHandlers : (Event -> Message) list,
        vmCommandHandlers : (VMCommand -> Message) list,
        vmEventHandlers : (VMEvent -> Message) list
    ) =
    let mutable _commandHandlers = commandHandlers
    let mutable _eventHandlers = eventHandlers
    let mutable _vmCommandHandlers = vmCommandHandlers
    let mutable _vmEventHandlers = vmEventHandlers

    member x.addCommandHandler commandHandler =
        _commandHandlers <- commandHandler :: _commandHandlers

    member x.addEventHandler eventHandler =
        _eventHandlers <- eventHandler :: _eventHandlers

    member x.addVMCommandHandler commandHandler =
        _vmCommandHandlers <- commandHandler :: _vmCommandHandlers

    member x.addVMEventHandler eventHandler =
        _vmEventHandlers <- eventHandler :: _vmEventHandlers

    member x.publish (message : Message) =
        match message with
        | :? Event as event ->
            for handle in _eventHandlers do
                handle event |> x.publish
        | :? Command as command ->
            for handle in _commandHandlers do
                handle command |> x.publish
        | :? VMEvent as event ->
            for handle in _vmEventHandlers do
                handle event |> x.publish
        | :? VMCommand as command ->
            for handle in _vmCommandHandlers do
                handle command |> x.publish
        | noMessage -> ()
