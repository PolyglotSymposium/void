namespace Void.Core

module CommandMode =
    let private handleEnter interpret buffer =
        match interpret { Language = "VoidScript"; Fragment = buffer} with
        | InterpretScriptFragmentResponse.Completed ->
            ("", Some Event.LineCommandCompleted)
        | InterpretScriptFragmentResponse.ParseFailed error ->
            ("", Event.ErrorOccurred error |> Some)
        | InterpretScriptFragmentResponse.ParseIncomplete ->
            (buffer + "\n", None)

    let private handleHotKey interpret buffer hotKey =
        match hotKey with
        | HotKey.Enter ->
            handleEnter interpret buffer
        | HotKey.Escape ->
            ("", Some Event.CommandEntryCancelled)
        | _ -> (buffer, None)
        
    let handle (interpret : RequestAPI.InterpretScriptFragment) buffer input =
        match input with
        | TextOrHotKey.Text text ->
            (buffer + text, None)
        | TextOrHotKey.HotKey hotKey ->
            handleHotKey interpret buffer hotKey
