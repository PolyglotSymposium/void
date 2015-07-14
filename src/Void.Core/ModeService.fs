namespace Void.Core

open NormalMode

[<RequireQualifiedAccess>]
type InputMode<'Output> =
    | KeyPresses of (KeyPress -> 'Output)
    | TextAndHotKeys of (TextOrHotKey -> 'Output)

type CommandModeInputHandler(interpret : RequestAPI.InterpretScriptFragment) =
    let handle = CommandMode.handle interpret
    let _buffer = ref ""

    member x.handleTextOrHotKey input =
        let updatedBuffer, message = handle !_buffer input
        _buffer := updatedBuffer
        message

    member x.handleHistoryEvent =
        Service.wrap _buffer CommandMode.handleHistoryEvent

type NormalModeInputHandler() =
    let _bindings = ref defaultBindings
    let mutable _state = noKeysYet

    member x.handleKeyPress keyPress =
        match parse !_bindings keyPress _state with
        | ParseResult.AwaitingKeyPress prevKeys ->
            _state <- prevKeys
            noMessage
        | ParseResult.CommandMatched command ->
            _state <- noKeysYet
            command :> Message

    member x.handleCommand =
        Service.wrap _bindings NormalMode.handleCommand

type VisualModeInputHandler() =
    member x.handleKeyPress whatever =
        notImplemented

type InsertModeInputHandler() =
    member x.handleTextOrHotKey whatever =
        notImplemented

type ModeNotImplementedYet_FakeInputHandler() =
    member x.handleAnything whatever =
        notImplemented

type ModeService
    (
        normalModeInputHandler : NormalModeInputHandler,
        commandModeInputHandler : CommandModeInputHandler,
        visualModeInputHandler : VisualModeInputHandler,
        insertModeInputHandler : InsertModeInputHandler,
        setInputMode : InputMode<Message> -> unit
    ) =
    let mutable _mode = Mode.Normal

    let inputHandlerFor =
        function
        | Mode.Normal ->
            InputMode.KeyPresses normalModeInputHandler.handleKeyPress
        | Mode.Command ->
            InputMode.TextAndHotKeys commandModeInputHandler.handleTextOrHotKey
        | Mode.Visual ->
            InputMode.KeyPresses visualModeInputHandler.handleKeyPress
        | Mode.Insert ->
            InputMode.TextAndHotKeys insertModeInputHandler.handleTextOrHotKey
        | _ ->
            ModeNotImplementedYet_FakeInputHandler().handleAnything
            |> InputMode.KeyPresses

    member x.handleEvent =
        function
        | CoreEvent.ErrorOccurred (Error.ScriptFragmentParseFailed _) -> 
            CoreCommand.ChangeToMode Mode.Normal :> Message // TODO or whatever mode we were in previously?
        | _ -> noMessage

    member x.handleCommandModeEvent =
        function
        | CommandMode.Event.CommandCompleted _ -> 
            if _mode = Mode.Command
            then CoreCommand.ChangeToMode Mode.Normal :> Message // TODO or whatever mode we were in previously?
            else noMessage
        | CommandMode.Event.EntryCancelled ->
            CoreCommand.ChangeToMode Mode.Normal :> Message // TODO or whatever mode we were in previously?
        | _ -> noMessage

    member x.handleCommand =
        function
        | CoreCommand.InitializeVoid ->
            setInputMode <| inputHandlerFor _mode
            CoreEvent.ModeSet _mode :> Message
        | CoreCommand.ChangeToMode mode ->
            let change = { From = _mode; To = mode }
            _mode <- change.To
            setInputMode <| inputHandlerFor change.To
            CoreEvent.ModeChanged change :> Message
        | _ -> noMessage

    member x.subscribe (subscribeHandler : SubscribeToBus) =
        subscribeHandler.subscribe x.handleCommandModeEvent
        subscribeHandler.subscribe x.handleEvent
        subscribeHandler.subscribe x.handleCommand
        subscribeHandler.subscribe commandModeInputHandler.handleHistoryEvent
        subscribeHandler.subscribe normalModeInputHandler.handleCommand