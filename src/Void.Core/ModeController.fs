namespace Void.Core

open NormalMode

[<RequireQualifiedAccess>]
type InputMode<'Output> =
    | KeyPresses of (KeyPress -> 'Output)
    | TextAndHotKeys of (TextOrHotKey -> 'Output)

type CommandModeInputHandler(interpret : RequestAPI.InterpretScriptFragment) =
    let handle = CommandMode.handle interpret
    let mutable _buffer = ""

    member x.handleTextOrHotKey input =
        let updatedBuffer, maybeEvent = handle _buffer input
        _buffer <- updatedBuffer
        match maybeEvent with
        | Some event -> Command.PublishEvent event
        | None -> Command.Noop

type NormalModeInputHandler() =
    let mutable _bindings = defaultBindings
    let mutable _state = noKeysYet

    member x.handleKeyPress keyPress =
        match parse _bindings keyPress _state with
        | ParseResult.AwaitingKeyPress prevKeys ->
            _state <- prevKeys
            Command.Noop
        | ParseResult.Command command ->
            _state <- noKeysYet
            command

type VisualModeInputHandler() =
    member x.handleKeyPress whatever =
        notImplemented

type InsertModeInputHandler() =
    member x.handleTextOrHotKey whatever =
        notImplemented

type ModeNotImplementedYet_FakeInputHandler() =
    member x.handleAnything whatever =
        notImplemented

type ModeController
    (
        normalModeInputHandler : NormalModeInputHandler,
        commandModeInputHandler : CommandModeInputHandler,
        visualModeInputHandler : VisualModeInputHandler,
        insertModeInputHandler : InsertModeInputHandler,
        setInputMode : InputMode<Command> -> unit
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
            then Command.ChangeToMode Mode.Normal // TODO or whatever mode we were in previously?
            else Command.Noop
        | Event.ErrorOccurred (Error.ScriptFragmentParseFailed _) -> 
            Command.ChangeToMode Mode.Normal // TODO or whatever mode we were in previously?
        | _ -> Command.Noop

    member x.handleCommand command =
        match command with
        | Command.InitializeVoid ->
            setInputMode <| inputHandlerFor _mode
            Command.PublishEvent <| Event.ModeSet _mode
        | Command.ChangeToMode mode ->
            let change = { From = _mode; To = mode }
            _mode <- change.To
            setInputMode <| inputHandlerFor change.To
            Command.PublishEvent <| Event.ModeChanged change
        | _ -> Command.Noop
