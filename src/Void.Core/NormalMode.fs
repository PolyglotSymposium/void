namespace Void.Core
open System.Collections.Generic

type NormalBindings = Map<KeyPress list, CommandMessage>

module NormalMode =
    type ParseResult =
        | AwaitingKeyPress of KeyPress list
        | Command of CommandMessage

    let defaultBindings = // TODO split out into "Vim default bindings" and "Void default bindings"
        [
            [KeyPress.Semicolon], CoreCommand.ChangeToMode Mode.Command :> CommandMessage
            [KeyPress.ControlC], CoreCommand.Yank :> CommandMessage
            [KeyPress.ControlV], CoreCommand.Put :> CommandMessage
            [KeyPress.ControlB], CoreCommand.ChangeToMode Mode.VisualBlock :> CommandMessage
            [KeyPress.ControlL], CoreCommand.Redraw :> CommandMessage
            [KeyPress.ShiftZ; KeyPress.ShiftQ], CoreCommand.QuitWithoutSaving :> CommandMessage
            [KeyPress.ShiftZ; KeyPress.ShiftA], CoreCommand.QuitAll :> CommandMessage
            [KeyPress.G; KeyPress.Q; KeyPress.Q], CoreCommand.FormatCurrentLine :> CommandMessage
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