namespace Void.Core

module CommandMode =
    let private handleEnter interpret buffer =
        match interpret { Language = "VoidScript"; Fragment = buffer} with
        | InterpretScriptFragmentResponse.Completed ->
            ("", Event.LineCommandCompleted :> Message)
        | InterpretScriptFragmentResponse.ParseFailed error ->
            ("", Event.ErrorOccurred error :> Message)
        | InterpretScriptFragmentResponse.ParseIncomplete ->
            (buffer + "\n", noMessage)

    let private handleHotKey interpret buffer hotKey =
        match hotKey with
        | HotKey.Enter ->
            handleEnter interpret buffer
        | HotKey.Escape ->
            ("", Event.CommandEntryCancelled :> Message)
        | HotKey.Backspace ->
            (buffer.Remove(buffer.Length - 1), Event.CommandMode_CharacterBackspaced :> Message)
        | _ -> (buffer, noMessage)

    let handle (interpret : RequestAPI.InterpretScriptFragment) buffer input =
        match input with
        | TextOrHotKey.Text text ->
            (buffer + text, Event.CommandMode_TextAppended text :> Message)
        | TextOrHotKey.HotKey hotKey ->
            handleHotKey interpret buffer hotKey
