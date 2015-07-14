namespace Void.Core.Spec

open Void.Core
open Void.Core.NormalMode
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Parsing normal mode commands``() = 
    let bindingsForTest = // TODO split out into "Vim default bindings" and "Void default bindings"
        [
            [KeyPress.Semicolon], CoreCommand.ChangeToMode Mode.Command :> CommandMessage
            [KeyPress.ShiftZ; KeyPress.ShiftQ], CoreCommand.QuitWithoutSaving :> CommandMessage
            [KeyPress.ShiftZ; KeyPress.ShiftA], CoreCommand.QuitAll :> CommandMessage
            [KeyPress.G; KeyPress.Q; KeyPress.Q], CoreCommand.FormatCurrentLine :> CommandMessage
        ] |> Map.ofList

    member x.parse keyPress prevKeys =
        parse bindingsForTest keyPress prevKeys

    [<Test>]
    member x.``Hitting escape clears any previous keys``() =
        x.parse KeyPress.Escape [KeyPress.Q; KeyPress.G] |> should equal (ParseResult.AwaitingKeyPress [])

    [<Test>]
    member x.``After receiving a single key press should translate it into a command``() =
        x.parse KeyPress.Semicolon [] |> should equal (ParseResult.Command (CoreCommand.ChangeToMode Mode.Command :> CommandMessage))

    [<Test>]
    member x.``After receiving a key press with no match should be waiting another key press``() =
        x.parse KeyPress.ShiftZ [] |> should equal (ParseResult.AwaitingKeyPress [KeyPress.ShiftZ])

    [<Test>]
    member x.``After receiving two key presses that together match should translate them into a command``() =
        x.parse KeyPress.ShiftQ [KeyPress.ShiftZ] |> should equal (ParseResult.Command CoreCommand.QuitWithoutSaving)

    [<Test>]
    member x.``After receiving three key presses that together match should translate them into a command``() =
        x.parse KeyPress.Q [KeyPress.Q; KeyPress.G] |> should equal (ParseResult.Command CoreCommand.FormatCurrentLine)
