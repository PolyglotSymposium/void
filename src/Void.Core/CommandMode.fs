namespace Void.Core

module CommandMode =
    let handle (interpret : RequestAPI.InterpretScriptFragment) buffer input =
        match input with
        | TextOrHotKey.Text text ->
            (buffer + text, None)
        | TextOrHotKey.HotKey hotKey ->
            match hotKey with
            | HotKey.Enter ->
                match interpret { Language = "VoidScript"; Fragment = buffer} with
                | InterpretScriptFragmentResponse.Completed ->
                    ("", Some Event.LineCommandCompleted)
                | InterpretScriptFragmentResponse.ParseFailed error ->
                    ("", Event.ErrorOccurred error |> Some)
                | InterpretScriptFragmentResponse.ParseIncomplete ->
                    (buffer + "\n", None)
            | HotKey.Escape ->
                ("", Some Event.CommandEntryCancelled)
            | _ -> (buffer, None)
