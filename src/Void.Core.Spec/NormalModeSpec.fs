namespace Void.Core.Spec

open Void.Core
open Void.Core.NormalMode
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Parsing normal mode commands``() = 
    member x.parse keyPress prevKeys =
        parse defaultBindings keyPress prevKeys

    [<Test>]
    member x.``Hitting escape clears any previous keys``() =
        x.parse KeyPress.Escape [KeyPress.Q; KeyPress.G] |> should equal (ParseResult.AwaitingKeyPress [])

    [<Test>]
    member x.``After receiving a single key press should translate it into a command``() =
        x.parse KeyPress.Semicolon [] |> should equal (ParseResult.Command <| CoreCommand.ChangeToMode Mode.Command)

    [<Test>]
    member x.``After receiving a key press with no match should be waiting another key press``() =
        x.parse KeyPress.ShiftZ [] |> should equal (ParseResult.AwaitingKeyPress [KeyPress.ShiftZ])

    [<Test>]
    member x.``After receiving two key presses that together match should translate them into a command``() =
        x.parse KeyPress.ShiftQ [KeyPress.ShiftZ] |> should equal (ParseResult.Command CoreCommand.QuitWithoutSaving)

    [<Test>]
    member x.``After receiving three key presses that together match should translate them into a command``() =
        x.parse KeyPress.Q [KeyPress.Q; KeyPress.G] |> should equal (ParseResult.Command CoreCommand.FormatCurrentLine)
