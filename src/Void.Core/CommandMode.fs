namespace Void.Core

module CommandMode =
    open Void.Util

    [<RequireQualifiedAccess>]
    type Event =
        | EntryCancelled
        | CharacterBackspaced
        | TextAppended of string
        | CommandCompleted of string
        interface EventMessage

    let private handleEnter interpret buffer =
        match interpret { Language = "VoidScript"; Fragment = buffer} with
        | InterpretScriptFragmentResponse.Completed ->
            ("", Event.CommandCompleted buffer :> Message)
        | InterpretScriptFragmentResponse.ParseFailed error ->
            ("", CoreEvent.ErrorOccurred error :> Message)
        | InterpretScriptFragmentResponse.ParseIncomplete ->
            (buffer + System.Environment.NewLine, noMessage)

    let private handleHotKey interpret buffer hotKey =
        let cancelled = ("", Event.EntryCancelled :> Message)
        match hotKey with
        | HotKey.Enter ->
            handleEnter interpret buffer
        | HotKey.Escape ->
            cancelled
        | HotKey.Backspace ->
            if buffer = ""
            then cancelled
            else (StringUtil.backspace buffer, Event.CharacterBackspaced :> Message)
        | HotKey.ArrowUp ->
            (buffer, CommandHistoryCommand.MoveToPreviousCommand :> Message)
        | HotKey.ArrowDown ->
            (buffer, CommandHistoryCommand.MoveToNextCommand :> Message)
        | _ -> (buffer, noMessage)

    let handle (interpret : RequestAPI.InterpretScriptFragment) buffer input =
        match input with
        | TextOrHotKey.Text text ->
            (buffer + text, Event.TextAppended text :> Message)
        | TextOrHotKey.HotKey hotKey ->
            handleHotKey interpret buffer hotKey
