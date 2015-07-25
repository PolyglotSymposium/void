namespace Void.Core
open System.Collections.Generic

type NormalBindings = Map<KeyPress list, CommandMessage>

module NormalMode =
    let emptyBindings =
        Map.empty<KeyPress list, CommandMessage>

    [<RequireQualifiedAccess>]
    type Command =
        | Bind of KeyPress list * CommandMessage
        interface CommandMessage

    [<RequireQualifiedAccess>]
    type Event =
        | KeysBoundToCommand
        interface EventMessage

    [<RequireQualifiedAccess>]
    type ParseResult =
        | AwaitingKeyPress of KeyPress list
        | CommandMatched of CommandMessage

    let noKeysYet =
        List.empty<KeyPress>

    let bind (bindings : NormalBindings) keyPresses command =
        bindings.Add(keyPresses, command)

    let parse (bindings : NormalBindings) keyPress prevKeys =
        if keyPress = KeyPress.Escape then
            ParseResult.AwaitingKeyPress noKeysYet
        else
            let keyPresses = keyPress :: prevKeys
            let inBindOrder = List.rev keyPresses
            if bindings.ContainsKey inBindOrder then
                bindings.Item inBindOrder |> ParseResult.CommandMatched
            else
                ParseResult.AwaitingKeyPress keyPresses

    let handleCommand bindings command =
        match command with
        | Command.Bind (keyPresses, bindToCommand) ->
            bind bindings keyPresses bindToCommand, Event.KeysBoundToCommand :> Message

    type InputHandler() =
        let _bindings = ref emptyBindings
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
            Service.wrap _bindings handleCommand
