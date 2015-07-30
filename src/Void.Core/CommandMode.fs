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

    let handleGetCurrentCommandLanguageResponse buffer response =
        {
            Language = response.CurrentCommandLanguage
            Fragment = !buffer
        } :> Message

    let handleNoResponseToGetCurrentCommandLanguage buffer (msg : NoResponseToRequest<GetCurrentCommandLanguageRequest>) =
        {
            Language = "VoidScript"
            Fragment = !buffer
        } :> Message

    let private handleHotKey buffer hotKey =
        let cancelled = ("", Event.EntryCancelled :> Message)
        match hotKey with
        | HotKey.Enter ->
            requestLanguageForInterpreting buffer
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

    let handle buffer input =
        match input with
        | TextOrHotKey.Text text ->
            buffer + text, Event.TextAppended text :> Message
        | TextOrHotKey.HotKey hotKey ->
            handleHotKey buffer hotKey

    let handleHistoryEvent buffer event =
        match event with
        | CommandHistoryEvent.MovedToCommand command ->
            (command, Event.TextReplaced command :> Message)
        | CommandHistoryEvent.MovedToEmptyCommand ->
            ("", Event.TextReplaced "" :> Message)
        | _ ->
            (buffer, noMessage)

    let handleInterpretFragmentResponse buffer response =
        match response with
        | InterpretScriptFragmentResponse.Completed ->
            ("", Event.CommandCompleted buffer :> Message)
        | InterpretScriptFragmentResponse.ParseFailed error ->
            ("", CoreEvent.ErrorOccurred error :> Message)
        | InterpretScriptFragmentResponse.ParseIncomplete ->
            (buffer + System.Environment.NewLine, noMessage)

    let handleNoResponseToInterpretFragmentRequest _ (msg : NoResponseToRequest<InterpretScriptFragmentRequest>) =
        "", CoreEvent.ErrorOccurred <| Error.UnableToInterpretLanguage msg.Request.Language :> Message

    type InputHandler() =
        let _buffer = ref ""

        member x.handleTextOrHotKey input =
            let updatedBuffer, message = handle !_buffer input
            _buffer := updatedBuffer
            message

        member x.subscribe (subscribeHandler : SubscribeToBus) =
            subscribeHandler.subscribe <| Service.wrap _buffer handleHistoryEvent
            //subscribeHandler.subscribeToResponse <| Service.wrap _buffer handleInterpretFragmentResponse
            //subscribeHandler.subscribeToResponse <| handleGetCurrentCommandLanguageResponse _buffer
            subscribeHandler.subscribe <| Service.wrap _buffer handleNoResponseToInterpretFragmentRequest
            subscribeHandler.subscribe <| handleNoResponseToGetCurrentCommandLanguage _buffer
