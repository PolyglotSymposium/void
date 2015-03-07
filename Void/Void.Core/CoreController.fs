namespace Void.Core

type CoreController() =
    let mutable _messages = []
    let mutable _buffer = Buffer.empty

    member x.handleCommand command =
        match command with
        | Command.ViewTestBuffer ->
            _buffer <- Buffer.testFile
            // TODO Should controllers create the events?...
            Event.BufferLoadedIntoWindow _buffer |> Command.PublishEvent
        | _ -> Command.Noop

    member x.handleEvent event =
        match event with
        | Event.ErrorOccurred error ->
            // TODO ...Or should the underlying model create the events?
            let messages, event = Messages.addError error _messages
            _messages <- messages
            Command.PublishEvent event
        | _ -> Command.Noop

