namespace Void.Core

module CommandMode =
    let handle (interpret : RequestAPI.InterpretScriptFragment) buffer input =
        match input with
        | TextOrHotKey.Text text ->
            (buffer + text, Command.Noop)
        | TextOrHotKey.HotKey hotKey ->
            match hotKey with
            | HotKey.Enter ->
                match interpret { Language = "VoidScript"; Fragment = buffer} with
                | InterpretScriptFragmentResponse.Completed ->
                    ("", Command.ChangeToMode Mode.Normal) // TODO but what if the command itself changed modes? etc
                | InterpretScriptFragmentResponse.ParseFailed message ->
                    ("", Event.ErrorOccurred message |> Command.PublishEvent)
                | InterpretScriptFragmentResponse.ParseIncomplete ->
                    (buffer + "\n", Command.Noop)
            | _ -> (buffer, Command.Noop)
