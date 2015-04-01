namespace Void.Core

open NormalMode

[<RequireQualifiedAccess>]
type InputMode<'Output> =
    | KeyPresses of (KeyPress -> 'Output)
    | TextAndHotKeys of (TextOrHotKey -> 'Output)

type CommandModeInputHandler() =
    member x.handleTextOrHotKey input =
        match input with
        | TextOrHotKey.Text text -> ()
        | TextOrHotKey.HotKey hotKey -> ()
        Command.Noop

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

type ModeController(setInputMode : InputMode<Command> -> unit) =
    let mutable _mode = Mode.Normal

    let inputHandlerFor mode =
        match mode with
        | Mode.Normal ->
            NormalModeInputHandler().handleKeyPress
            |> InputMode.KeyPresses
        | Mode.Command ->
            CommandModeInputHandler().handleTextOrHotKey
            |> InputMode.TextAndHotKeys
        | Mode.Visual ->
            VisualModeInputHandler().handleKeyPress
            |> InputMode.KeyPresses
        | Mode.Insert ->
            InsertModeInputHandler().handleTextOrHotKey
            |> InputMode.TextAndHotKeys
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
