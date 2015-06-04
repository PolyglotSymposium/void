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
        let bar, msg = Event.ModeChanged { From = Mode.Normal; To = Mode.Command }
                       |> CommandBar.handleEvent CommandBar.hidden
        bar |> should equal CommandBar.visibleButEmpty
        msg |> should equal (VMEvent.CommandBar_Displayed bar)

    [<Test>]
    member x.``should disappear when command entry is cancelled``() =
        let bar, msg = CommandBar.handleEvent CommandBar.visibleButEmpty Event.CommandEntryCancelled
        bar |> should equal CommandBar.hidden
        msg |> should equal (VMEvent.CommandBar_Hidden CommandBar.hidden)

    [<Test>]
    member x.``should remove the last character when it is backspaced``() =
        let bar, msg = CommandBar.handleEvent visibleQui5 Event.CommandMode_CharacterBackspaced
        bar |> should equal visibleQui
        msg |> should equal (VMEvent.CommandBar_CharacterBackspacedFromLine { Row = 0; Column = 4 })

    [<Test>]
    member x.``should reflow the text when backspace results in removal of line``() =
        let bar, msg = CommandBar.handleEvent oneCharLongerThanOneLine Event.CommandMode_CharacterBackspaced
        bar |> should equal oneFullLine
        msg |> should equal (VMEvent.CommandBar_TextReflowed oneFullLine)

    [<Test>]
    member x.``should add characters when text is appended``() =
        let bar, msg = CommandBar.handleEvent visibleQui <| Event.CommandMode_TextAppended "5"
        bar |> should equal visibleQui5
        msg |> should equal (VMEvent.CommandBar_TextAppendedToLine { LeftMostCell = { Row = 0; Column = 4 }; Text = "5" })

    [<Test>]
    member x.``should reflow the text when an added character results in a second line``() =
        let bar, msg = CommandBar.handleEvent oneFullLine <| Event.CommandMode_TextAppended "x"
        bar |> should equal oneCharLongerThanOneLine
        msg |> should equal (VMEvent.CommandBar_TextReflowed oneCharLongerThanOneLine)

    [<Test>]
    member x.``should reflow the text when an added character results in a third line``() =
        let bar, msg = CommandBar.handleEvent twoFullLines <| Event.CommandMode_TextAppended "x"
        bar |> should equal oneCharLongerThanTwoLines
        msg |> should equal (VMEvent.CommandBar_TextReflowed oneCharLongerThanTwoLines)

    [<Test>]
    [<Ignore("TODO get this edge case working")>]
    member x.``should reflow the text multiple added characters results in another line``() =
        let bar, msg = CommandBar.handleEvent oneCharShorterThanFullLine <| Event.CommandMode_TextAppended "xx"
        bar |> should equal oneCharLongerThanOneLine
        msg |> should equal (VMEvent.CommandBar_TextReflowed oneCharLongerThanOneLine)
