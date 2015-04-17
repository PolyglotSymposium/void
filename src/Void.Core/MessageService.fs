namespace Void.Core

type MessageService() =
    let mutable _messages = []

    member x.handleEvent event =
        match event with
        | Event.ErrorOccurred error ->
            // TODO ...Or should the underlying model create the events?
            let messages, event = Messages.addError error _messages
            _messages <- messages
            Command.PublishEvent event
        | _ -> Command.Noop

    member x.handleCommand command =
        match command with
        | Command.ShowMessages ->
            Command.Display <| Displayable.Messages _messages
        | _ -> Command.Noop

