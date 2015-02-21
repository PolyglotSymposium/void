namespace Void.Core

open NormalMode

type NormalModeController() =
    let mutable _bindings = defaultBindings
    let mutable _state = noKeysYet

    member x.handle keyPress =
        match parse _bindings keyPress _state with
        | ParseResult.AwaitingKeyPress prevKeys ->
            _state <- prevKeys
            Command.Noop
        | ParseResult.Command command ->
            _state <- noKeysYet
            command
