namespace Void.Core.Spec

open Void.Core
open Void.Core.NormalModeBindings
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
    let startTime =
        System.DateTime.Now
    let ``less than a second later`` =
        startTime.AddSeconds 0.9
    let ``a second later`` =
        startTime.AddSeconds 1.0
    let ``more than a second later`` =
        startTime.AddSeconds 1.1
    let startState =
        { empty with Bindings = bindingsForTest }
    let laterState = 
        { startState with ExpireTime = ``a second later`` }
    let escape =
        { Key = KeyPress.Escape; Timestamp = startTime }
    let semicolon =
        { Key = KeyPress.Semicolon; Timestamp = startTime }
    let shiftA =
        { Key = KeyPress.ShiftA; Timestamp = startTime }
    let shiftQ =
        { Key = KeyPress.ShiftQ; Timestamp = startTime }
    let shiftZ =
        { Key = KeyPress.ShiftZ; Timestamp = startTime }
    let q =
        { Key = KeyPress.Q; Timestamp = startTime }

    [<Test>]
    member x.``Hitting escape clears any previous keys``() =
        let state = { startState with KeyPressBuffer = [KeyPress.Q; KeyPress.G] }
        handleKeyPress state escape
        |> should equal (startState, Event.KeyPressesCleared :> Message)

    [<Test>]
    member x.``After receiving a single key press should translate it into a command``() =
        handleKeyPress startState semicolon
        |> should equal (startState, CoreCommand.ChangeToMode Mode.Command :> Message)

    [<Test>]
    member x.``After receiving a key press with no match should be waiting another key press``() =
        handleKeyPress startState shiftZ
        |> should equal ({ laterState with KeyPressBuffer = [KeyPress.ShiftZ]}, Event.KeyPressRegistered KeyPress.ShiftZ :> Message)

    [<Test>]
    member x.``After receiving a two key press with no match should be waiting for a third key press``() =
        let state = { startState with KeyPressBuffer = [KeyPress.ShiftG] }
        handleKeyPress state shiftQ
        |> should equal ({ laterState with KeyPressBuffer = [KeyPress.ShiftQ; KeyPress.ShiftG]}, Event.KeyPressRegistered KeyPress.ShiftQ :> Message)

    [<Test>]
    member x.``After receiving two key presses that together match should translate them into a command``() =
        let state = { startState with KeyPressBuffer = [KeyPress.ShiftZ] }
        handleKeyPress state shiftQ
        |> should equal (startState, CoreCommand.QuitWithoutSaving :> Message)

    [<Test>]
    member x.``After receiving three key presses that together match should translate them into a command``() =
        let state = { startState with KeyPressBuffer = [KeyPress.Q; KeyPress.G] }
        handleKeyPress state q
        |> should equal (startState, CoreCommand.FormatCurrentLine :> Message)

    [<Test>]
    member x.``After receiving two key presses less than the timeout period apart should translate them into a command``() =
        let state = { startState with KeyPressBuffer = [KeyPress.ShiftZ] }
        handleKeyPress state { shiftQ with Timestamp = ``less than a second later`` }
        |> should equal (startState, CoreCommand.QuitWithoutSaving :> Message)

    [<Test>]
    member x.``After receiving two key presses more than the timeout period apart should throw the first away``() =
        let state = { laterState with KeyPressBuffer = [KeyPress.ShiftZ] }
        handleKeyPress state { shiftQ with Timestamp = ``more than a second later`` }
        |> should equal (
            {
                Bindings = bindingsForTest
                KeyPressBuffer = [KeyPress.ShiftQ]
                ExpireTime = ``more than a second later``.AddSeconds 1.0
            }, Event.ExpiredKeyPressReplacedWith KeyPress.ShiftQ :> Message)

    [<Test>]
    member x.``After receiving two key presses more than the timeout period apart should match the second one in isolation if necessary``() =
        let state = { laterState with KeyPressBuffer = [KeyPress.ShiftZ] }
        handleKeyPress state { semicolon with Timestamp = ``more than a second later`` }
        |> should equal (startState, CoreCommand.ChangeToMode Mode.Command :> Message)
