namespace Void.ViewModel

module NotifyUserOfEvent =
    open Void.Core

    let handleEvent event =
        match event with
        | Event.FileSaved path ->
            sprintf "\"%s\" written" path
            |> UserNotification.Output
            |> Command.AddNotification :> Message
        | _ -> noMessage