namespace Void.Core

open NormalMode

[<RequireQualifiedAccess>]
type InputMode<'Output> =
    | KeyPresses of (KeyPress -> 'Output)
    | TextAndHotKeys of (TextOrHotKey -> 'Output)

type CommandModeInputHandler(interpret : RequestAPI.InterpretScriptFragment) =
    let mutable _buffer = ""
    // TODO refactor... this is a mess
    member x.handleTextOrHotKey input =
        match input with
        | TextOrHotKey.Text text ->
            _buffer <- _buffer + text
            Command.Noop
        | TextOrHotKey.HotKey hotKey ->
            match hotKey with
            | HotKey.Enter ->
                match interpret { Language = "VoidScript"; Fragment = _buffer} with
                | InterpretScriptFragmentResponse.Completed ->
                    _buffer <- ""
                    Command.ChangeToMode Mode.Normal // TODO but what if the command itself changed modes? etc
                | InterpretScriptFragmentResponse.ParseFailed message ->
                    Event.ErrorOccurred message |> Command.PublishEvent
                | InterpretScriptFragmentResponse.ParseIncomplete ->
                    _buffer <- "\n"
                    Command.Noop
            | _ -> Command.Noop

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
