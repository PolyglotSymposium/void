namespace Void.ViewModel

module NotifyUserOfEvent =
    open Void.Core

    let handleEvent event =
        match event with
        | CoreEvent.FileSaved path ->
            sprintf "\"%s\" written" path
            |> UserNotification.Output
            |> CoreCommand.AddNotification :> Message
        | _ -> noMessage