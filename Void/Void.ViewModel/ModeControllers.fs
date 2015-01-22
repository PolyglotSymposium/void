namespace Void.ViewModel

open Void.Core

type NormalModeController() =
    let _bindings = NormalMode.defaultBindings
    let mutable _state = NormalMode.noKeysYet

    member x.handle keyPress =
        match NormalMode.parse _bindings keyPress _state with
        | NormalMode.ParseResult.AwaitingKeyPress prevKeys ->
            _state <- prevKeys
            Command.Noop
        | NormalMode.ParseResult.Command command ->
            command
