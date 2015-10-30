namespace Void.ViewModel.Spec

open Void.Core
open Void.ViewModel
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``The command mode command line (or command bar for short)``() = 
    let visibleQui = { CommandBar.visibleButEmpty with WrappedLines = ["qui"] }
    let visibleQui5 = { visibleQui with WrappedLines = ["qui5"] }
    let oneCharShorterThanFullLine = { CommandBar.visibleButEmpty with WrappedLines = ["xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"] }
    let oneFullLine = { CommandBar.visibleButEmpty with WrappedLines = ["xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"] }
    let oneCharLongerThanOneLine = { oneFullLine with WrappedLines = "x" :: oneFullLine.WrappedLines }
    let twoFullLines = { oneFullLine with WrappedLines = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" :: oneFullLine.WrappedLines }
    let oneCharLongerThanTwoLines = { twoFullLines with WrappedLines = "x" :: twoFullLines.WrappedLines }

    [<Test>]
    member x.``should display when entering command mode``() =
        let bar, msg = CoreEvent.ModeChanged { From = Mode.Normal; To = Mode.Command }
                       |> CommandBar.handleEvent CommandBar.hidden
        bar |> should equal CommandBar.visibleButEmpty
        msg |> should equal (CommandBar.Event.Displayed bar)

    [<Test>]
    member x.``should disappear when command entry is cancelled``() =
        let bar, msg = CommandBar.handleCommandModeEvent CommandBar.visibleButEmpty CommandMode.Event.EntryCancelled
        bar |> should equal CommandBar.hidden
        msg |> should equal (CommandBar.Event.Hidden CommandBar.hidden)

    [<Test>]
    member x.``should remove the last character when it is backspaced``() =
        let bar, msg = CommandBar.handleCommandModeEvent visibleQui5 CommandMode.Event.CharacterBackspaced
        bar |> should equal visibleQui
        msg |> should equal (CommandBar.Event.CharacterBackspacedFromLine { Row = 0<mRow>; Column = 4<mColumn> })

    [<Test>]
    member x.``should reflow the text when backspace results in removal of line``() =
        let bar, msg = CommandBar.handleCommandModeEvent oneCharLongerThanOneLine CommandMode.Event.CharacterBackspaced
        bar |> should equal oneFullLine
        msg |> should equal (CommandBar.Event.TextReflowed oneFullLine)

    [<Test>]
    member x.``should add characters when text is appended``() =
        let bar, msg = CommandBar.handleCommandModeEvent visibleQui <| CommandMode.Event.TextAppended "5"
        bar |> should equal visibleQui5
        msg |> should equal (CommandBar.Event.TextAppendedToLine { LeftMostCell = { Row = 0<mRow>; Column = 4<mColumn> }; Text = "5" })

    [<Test>]
    member x.``should reflow the text when an added character results in a second line``() =
        let bar, msg = CommandBar.handleCommandModeEvent oneFullLine <| CommandMode.Event.TextAppended "x"
        bar |> should equal oneCharLongerThanOneLine
        msg |> should equal (CommandBar.Event.TextReflowed oneCharLongerThanOneLine)

    [<Test>]
    member x.``should reflow the text when an added character results in a third line``() =
        let bar, msg = CommandBar.handleCommandModeEvent twoFullLines <| CommandMode.Event.TextAppended "x"
        bar |> should equal oneCharLongerThanTwoLines
        msg |> should equal (CommandBar.Event.TextReflowed oneCharLongerThanTwoLines)

    [<Test>]
    [<Ignore("TODO get this edge case working")>]
    member x.``should reflow the text multiple added characters results in another line``() =
        let bar, msg = CommandBar.handleCommandModeEvent oneCharShorterThanFullLine <| CommandMode.Event.TextAppended "xx"
        bar |> should equal oneCharLongerThanOneLine
        msg |> should equal (CommandBar.Event.TextReflowed oneCharLongerThanOneLine)
