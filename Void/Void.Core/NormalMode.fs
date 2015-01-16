namespace Void.Core
open System.Collections.Generic

type NormalBindings = Map<KeyPress list, Command>

module NormalMode =
    type ParseResult =
        | AwaitingKeyPress of KeyPress list
        | Command of Command

    let defaultBindings = 
        [
            [KeyPress.Semicolon], Command.ChangeToMode Mode.Command
            [KeyPress.ShiftZ; KeyPress.ShiftQ], Command.Quit
            [KeyPress.G; KeyPress.Q; KeyPress.Q], Command.FormatCurrentLine
        ] |> Map.ofList

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
                bindings.Item inBindOrder |> ParseResult.Command
            else
                ParseResult.AwaitingKeyPress keyPresses