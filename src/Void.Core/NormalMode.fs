namespace Void.Core
open System.Collections.Generic

type NormalBindings = Map<KeyPress list, CoreCommand>

module NormalMode =
    type ParseResult =
        | AwaitingKeyPress of KeyPress list
        | Command of CoreCommand

    let defaultBindings = // TODO split out into "Vim default bindings" and "Void default bindings"
        [
            [KeyPress.Semicolon], CoreCommand.ChangeToMode Mode.Command
            [KeyPress.ControlC], CoreCommand.Yank
            [KeyPress.ControlV], CoreCommand.Put
            [KeyPress.ControlB], CoreCommand.ChangeToMode Mode.VisualBlock
            [KeyPress.ControlL], CoreCommand.Redraw
            [KeyPress.ShiftZ; KeyPress.ShiftQ], CoreCommand.QuitWithoutSaving
            [KeyPress.ShiftZ; KeyPress.ShiftA], CoreCommand.QuitAll
            [KeyPress.G; KeyPress.Q; KeyPress.Q], CoreCommand.FormatCurrentLine
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