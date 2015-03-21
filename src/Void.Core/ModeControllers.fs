namespace Void.Core

open NormalMode

type ModeController() =
    let mutable _mode = Mode.Normal

    member x.handleCommand command =
        match command with
        | Command.InitializeVoid ->
            Command.PublishEvent <| Event.ModeSet _mode
        | Command.ChangeToMode mode ->
            _mode <- mode
            Command.PublishEvent <| Event.ModeSet _mode
        | _ -> Command.Noop

type NormalModeController() =
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
