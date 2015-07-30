namespace Void.ViewModel

module NotifyUserOfEvent =
    open Void.Core

    let handleEvent event =
        match event with
        | CoreEvent.FileSaved path ->
            sprintf "\"%s\" written" path
            |> UserNotification.Output
            |> CoreCommand.AddNotification :> Message
        (*| CoreEvent.BufferAdded (id, buffer) ->
            match buffer with // TODO I've got a wrong abstraction here
            | BufferType.File fileBuffer ->
                match fileBuffer.Filepath with
                | Some path ->
                    sprintf "\"%s\" [New file]" path
                    |> UserNotification.Output
                    |> CoreCommand.AddNotification :> Message
                | None -> noMessage
            | _ -> noMessage*)
        | _ -> noMessage

    module Service =
        let subscribe (bus : Bus) =
            bus.subscribe handleEvent
