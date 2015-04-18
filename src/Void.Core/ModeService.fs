namespace Void.Core

open NormalMode

[<RequireQualifiedAccess>]
type InputMode<'Output> =
    | KeyPresses of (KeyPress -> 'Output)
    | TextAndHotKeys of (TextOrHotKey -> 'Output)

type CommandModeInputHandler(interpret : RequestAPI.InterpretScriptFragment) =
    let handle = CommandMode.handle interpret
    let mutable _buffer = ""

    // This is so far the model of what I think most "Controllers" should look like
    member x.handleTextOrHotKey input =
        let updatedBuffer, message = handle _buffer input
        _buffer <- updatedBuffer
        message

type NormalModeInputHandler() =
    let mutable _bindings = defaultBindings
    let mutable _state = noKeysYet

    member x.handleKeyPress keyPress =
        match parse _bindings keyPress _state with
        | ParseResult.AwaitingKeyPress prevKeys ->
            _state <- prevKeys
            noMessage
        | ParseResult.Command command ->
            _state <- noKeysYet
            command :> Message

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

    let inputHandlerFor mode =
        match mode with
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

    member x.handleEvent event =
        match event with
        | Event.LineCommandCompleted -> 
            if _mode = Mode.Command
            then Command.ChangeToMode Mode.Normal :> Message // TODO or whatever mode we were in previously?
            else noMessage
        | Event.CommandEntryCancelled
        | Event.ErrorOccurred (Error.ScriptFragmentParseFailed _) -> 
            Command.ChangeToMode Mode.Normal :> Message // TODO or whatever mode we were in previously?
        | _ -> noMessage

    member x.handleCommand command =
        match command with
        | Command.InitializeVoid ->
            setInputMode <| inputHandlerFor _mode
            Event.ModeSet _mode :> Message
        | Command.ChangeToMode mode ->
            let change = { From = _mode; To = mode }
            _mode <- change.To
            setInputMode <| inputHandlerFor change.To
            Event.ModeChanged change :> Message
        | _ -> noMessage
