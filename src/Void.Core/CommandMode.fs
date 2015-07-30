namespace Void.Core

module CommandMode =
    open Void.Util

    [<RequireQualifiedAccess>]
    type Event =
        | EntryCancelled
        | CharacterBackspaced
        | TextAppended of string
        | TextReplaced of string
        | CommandCompleted of string
        interface EventMessage

    let private requestLanguageForInterpreting buffer =
        buffer, GetCurrentCommandLanguageRequest :> Message

    let interpretFragment buffer maybeResponse =
        {
            Language =
                match maybeResponse with
                | Some response -> response.CurrentCommandLanguage
                | None -> "VoidScript"
            Fragment = buffer
        }

    let handleInterpretFragmentResponse buffer maybeResponse =
        match maybeResponse with
        | Some response ->
            match response with
            | InterpretScriptFragmentResponse.Completed ->
                "", Event.CommandCompleted buffer :> Message
            | InterpretScriptFragmentResponse.ParseFailed error ->
                "", CoreEvent.ErrorOccurred error :> Message
            | InterpretScriptFragmentResponse.ParseIncomplete ->
                buffer + System.Environment.NewLine, noMessage
        | None ->
            "", CoreEvent.ErrorOccurred <| Error.NoInterpreter :> Message

    let interpret (requestSender : RequestSender) buffer =
        GetCurrentCommandLanguageRequest
        |> requestSender.makeRequest
        |> interpretFragment buffer
        |> requestSender.makeRequest
        |> handleInterpretFragmentResponse buffer

    let private handleHotKey requestSender buffer hotKey =
        let cancelled = ("", Event.EntryCancelled :> Message)
        match hotKey with
        | HotKey.Enter ->
            interpret requestSender buffer
        | HotKey.Escape ->
            cancelled
        | HotKey.Backspace ->
            if buffer = ""
            then cancelled
            else StringUtil.backspace buffer, Event.CharacterBackspaced :> Message
        | HotKey.ArrowUp ->
            buffer, CommandHistoryCommand.MoveToPreviousCommand :> Message
        | HotKey.ArrowDown ->
            buffer, CommandHistoryCommand.MoveToNextCommand :> Message
        | _ -> (buffer, noMessage)

    let handle requestSender buffer input =
        match input with
        | TextOrHotKey.Text text ->
            buffer + text, Event.TextAppended text :> Message
        | TextOrHotKey.HotKey hotKey ->
            handleHotKey requestSender buffer hotKey

    let handleHistoryEvent buffer event =
        match event with
        | CommandHistoryEvent.MovedToCommand command ->
            command, Event.TextReplaced command :> Message
        | CommandHistoryEvent.MovedToEmptyCommand ->
            "", Event.TextReplaced "" :> Message
        | _ ->
            buffer, noMessage

    type InputHandler(requestSender : RequestSender) =
        let _buffer = ref ""

        member x.handleTextOrHotKey input =
            let updatedBuffer, message = handle requestSender !_buffer input
            _buffer := updatedBuffer
            message

        member x.subscribe (bus : Bus) =
            bus.subscribe <| Service.wrap _buffer handleHistoryEvent
