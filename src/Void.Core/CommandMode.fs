namespace Void.Core

module CommandMode =
    open Void.Util

    let private handleEnter interpret buffer =
        match interpret { Language = "VoidScript"; Fragment = buffer} with
        | InterpretScriptFragmentResponse.Completed ->
            ("", CoreEvent.LineCommandCompleted :> Message)
        | InterpretScriptFragmentResponse.ParseFailed error ->
            ("", CoreEvent.ErrorOccurred error :> Message)
        | InterpretScriptFragmentResponse.ParseIncomplete ->
            (buffer + "\n", noMessage)

    let private handleHotKey interpret buffer hotKey =
        let cancelled = ("", CoreEvent.CommandEntryCancelled :> Message)
        match hotKey with
        | HotKey.Enter ->
            handleEnter interpret buffer
        | HotKey.Escape ->
            cancelled
        | HotKey.Backspace ->
            if buffer = ""
            then cancelled
            else (StringUtil.backspace buffer, CoreEvent.CommandMode_CharacterBackspaced :> Message)
        | _ -> (buffer, noMessage)

    let handle (interpret : RequestAPI.InterpretScriptFragment) buffer input =
        match input with
        | TextOrHotKey.Text text ->
            (buffer + text, CoreEvent.CommandMode_TextAppended text :> Message)
        | TextOrHotKey.HotKey hotKey ->
            handleHotKey interpret buffer hotKey
