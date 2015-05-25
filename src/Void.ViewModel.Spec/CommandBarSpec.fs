namespace Void.ViewModel.Spec

open Void.Core
open Void.ViewModel
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``The command mode command line (or command bar for short)``() = 
    let visibleQui = {
        Width = 80
        Prompt = Visible CommandBarPrompt.VoidDefault
        WrappedLines = ["qui"]
    }
    let visibleQui5 = { visibleQui with WrappedLines = ["qui5"] }
    let oneFullLine = {
        Width = 80
        Prompt = Visible CommandBarPrompt.VoidDefault
        WrappedLines = [string('x', 79)]
    }
    let oneCharLongerThanOneLine = { oneFullLine with WrappedLines = "x" :: oneFullLine.WrappedLines }

    [<Test>]
    member x.``should display when entering command mode``() =
        let bar, msg = Event.ModeChanged { From = Mode.Normal; To = Mode.Command }
                       |> CommandBarV2.handleEvent CommandBarV2.hidden
        bar |> should equal CommandBarV2.visibleButEmpty
        msg |> should equal (VMEvent.CommandBar_Displayed bar)

    [<Test>]
    member x.``should disappear when command entry is cancelled``() =
        let bar, msg = CommandBarV2.handleEvent CommandBarV2.visibleButEmpty Event.CommandEntryCancelled
        bar |> should equal CommandBarV2.hidden
        msg |> should equal VMEvent.CommandBar_Hidden

    [<Test>]
    member x.``should remove the last character when it is backspaced``() =
        let bar, msg = CommandBarV2.handleEvent visibleQui5 Event.CommandMode_CharacterBackspaced
        bar |> should equal visibleQui
        msg |> should equal (VMEvent.CommandBar_CharacterBackspacedFromLine { Row = 0; Column = 4 })

    [<Test>]
    member x.``should reflow the text when backspace results in removal of line``() =
        let bar, msg = CommandBarV2.handleEvent oneCharLongerThanOneLine Event.CommandMode_CharacterBackspaced
        bar |> should equal oneFullLine
        msg |> should equal (VMEvent.CommandBar_TextReflowed oneFullLine)
