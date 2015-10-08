namespace Void.ViewModel.Spec

open Void.ViewModel
open Void.Core
open Void.Core.CellGrid
open System
open System.Linq
open NUnit.Framework
open FsUnit

[<AutoOpen>]
module Assertions =
    let shouldEqual expected actual =
        printfn "Expected: %A" expected
        printfn "Actual: %A" actual
        should equal expected actual

[<TestFixture>]
type ``Constructing a buffer view model from a sequence of text lines``() = 
    let asViewModelBuffer = Window.bufferFrom { Rows = 25<mRow>; Columns = 80<mColumn> }

    [<Test>]
    member x.``should create an empty buffer view model from an empty buffer``() =
        asViewModelBuffer Seq.empty
        |> should equal []

    [<Test>]
    member x.``for one line, shorter than the window width, should create a buffer with one line``() =
        seq { yield "line 1" }
        |> asViewModelBuffer 
        |> should equal ["line 1"]

    [<Test>]
    member x.``for one line, with length equal to the window width, should create a buffer with one line``() =
        seq { yield String('X', 80) }
        |> asViewModelBuffer 
        |> should equal [String('X', 80)]

    [<Test>]
    member x.``for one line, longer than the window width, should truncate the visible part of the line``() =
        // TODO this is the behavior when Vim's wrap option is set to nowrap
        seq { yield String('x', 81) }
        |> asViewModelBuffer 
        |> should equal [String('x', 80)]

    [<Test>]
    member x.``for a buffer which has more lines than the window has height, should create a buffer view model to fill the window size``() =
        Enumerable.Repeat("line", 26)
        |> asViewModelBuffer 
        |> should equal (Seq.toList <| Enumerable.Repeat("line", 25))

[<TestFixture>]
type ``Scrolling (by line)``() = 
    let buffer = ref [""]
    let requestSender =
        let handleRequest (request : GetWindowContentsRequest) =
            {
                FirstLineNumber = request.StartingAtLine
                RequestedContents = Seq.skip (request.StartingAtLine/1<mLine> - 1) !buffer
            } : GetWindowContentsResponse
        let bus = Messaging.newBus()
        bus.subscribeToRequest handleRequest
        bus :> RequestSender

    let scroll window movement =
        VMCommand.Scroll movement
        |> Window.handleVMCommand requestSender window

    [<SetUp>]
    member x.``Set up``() =
        buffer := ["a"; "b"; "c"; "d"; "e"; "f"]

    [<Test>]
    member x.``up when we are already at the top of the file should do nothing``() =
        let windowBefore = { Window.defaultWindowView with Buffer = !buffer }

        Move.backward By.line 3
        |> scroll windowBefore 
        |> shouldEqual (windowBefore, noMessage)

    [<Test>]
    member x.``up one line when the top line is two should work``() =
        let windowBefore = { Window.defaultWindowView with Buffer = ["b"; "c"; "d"; "e"; "f"]; TopLineNumber = 2<mLine> }
        let windowAfter = { windowBefore with TopLineNumber = 1<mLine>; Buffer = !buffer }

        Move.backward By.line 1
        |> scroll windowBefore 
        |> shouldEqual (windowAfter, Window.Event.ContentsUpdated windowAfter :> Message)

    [<Test>]
    member x.``up three lines when the top line is four should go to the top of the file``() =
        let windowBefore = { Window.defaultWindowView with Buffer = ["d"; "e"; "f"]; TopLineNumber = 4<mLine> }
        let windowAfter = { windowBefore with TopLineNumber = 1<mLine>; Buffer = !buffer }

        Move.backward By.line 3
        |> scroll windowBefore 
        |> shouldEqual (windowAfter, Window.Event.ContentsUpdated windowAfter :> Message)

    [<Test>]
    member x.``up four lines when the top line is three should go to the top of the file``() =
        let windowBefore = { Window.defaultWindowView with Buffer = ["d"; "e"; "f"]; TopLineNumber = 3<mLine> }
        let windowAfter = { windowBefore with TopLineNumber = 1<mLine>; Buffer = !buffer }

        Move.backward By.line 4
        |> scroll windowBefore 
        |> shouldEqual (windowAfter, Window.Event.ContentsUpdated windowAfter :> Message)

    [<Test>]
    member x.``up when the buffer is empty should do nothing``() =
        buffer := []
        let windowBefore = Window.defaultWindowView

        Move.backward By.line 1
        |> scroll windowBefore 
        |> shouldEqual (windowBefore, noMessage)

    [<Test>]
    member x.``down when the buffer is empty should do nothing``() =
        buffer := []
        let windowBefore = Window.defaultWindowView

        Move.forward By.line 1
        |> scroll windowBefore 
        |> shouldEqual (windowBefore, noMessage)

    [<Test>]
    member x.``down when only the last line of the buffer is showing should do nothing``() =
        let windowBefore = { Window.defaultWindowView with TopLineNumber = 6<mLine>; Buffer = ["f"] }

        Move.forward By.line 1
        |> scroll windowBefore 
        |> shouldEqual (windowBefore, noMessage)

    [<Test>]
    member x.``down multiple lines from the top``() =
        let windowBefore = { Window.defaultWindowView with Buffer = !buffer }
        let windowAfter = { windowBefore with TopLineNumber = 4<mLine>; Buffer = ["d"; "e"; "f"] }

        Move.forward By.line 3
        |> scroll windowBefore 
        |> shouldEqual (windowAfter, Window.Event.ContentsUpdated windowAfter :> Message)

[<TestFixture>]
type ``Scrolling (by half screen)``() = 
    let buffer = ref [""]
    let requestSender =
        let handleRequest (request : GetWindowContentsRequest) =
            {
                FirstLineNumber = request.StartingAtLine
                RequestedContents = Seq.skip (request.StartingAtLine/1<mLine> - 1) !buffer
            } : GetWindowContentsResponse
        let bus = Messaging.newBus()
        bus.subscribeToRequest handleRequest
        bus :> RequestSender

    let scrollHalf window movement =
        VMCommand.ScrollHalf movement
        |> Window.handleVMCommand requestSender window

    [<SetUp>]
    member x.``Set up``() =
        buffer := ["a"; "b"; "c"; "d"; "e"; "f"]

    [<Test>]
    member x.``up when we are already at the top of the file should do nothing``() =
        let windowBefore = { Window.defaultWindowView with Buffer = !buffer }

        Move.backward vmBy.screenHeight 1
        |> scrollHalf windowBefore 
        |> shouldEqual (windowBefore, noMessage)

    [<Test>]
    member x.``up half a screen height when the top line is two should go to the top of the file``() =
        let windowBefore = { Window.defaultWindowView with Buffer = ["b"; "c"; "d"; "e"; "f"]; TopLineNumber = 2<mLine> }
        let windowAfter = { windowBefore with TopLineNumber = 1<mLine>; Buffer = !buffer }

        Move.backward vmBy.screenHeight 1
        |> scrollHalf windowBefore 
        |> shouldEqual (windowAfter, Window.Event.ContentsUpdated windowAfter :> Message)

    [<Test>]
    member x.``up when the buffer is empty should do nothing``() =
        buffer := []
        let windowBefore = Window.defaultWindowView

        Move.backward vmBy.screenHeight 1
        |> scrollHalf windowBefore 
        |> shouldEqual (windowBefore, noMessage)

    [<Test>]
    member x.``down when the buffer is empty should do nothing``() =
        buffer := []
        let windowBefore = Window.defaultWindowView

        Move.forward vmBy.screenHeight 1
        |> scrollHalf windowBefore 
        |> shouldEqual (windowBefore, noMessage)

    [<Test>]
    member x.``down when only the last line of the buffer is showing should do nothing``() =
        let windowBefore = { Window.defaultWindowView with TopLineNumber = 6<mLine>; Buffer = ["f"] }

        Move.forward vmBy.screenHeight 1
        |> scrollHalf windowBefore 
        |> shouldEqual (windowBefore, noMessage)

    [<Test>]
    member x.``down when less than half a screen is showing should leave last line showing``() =
        let windowBefore = { Window.defaultWindowView with Buffer = !buffer }
        let windowAfter = { windowBefore with TopLineNumber = 6<mLine>; Buffer = ["f"] }

        Move.forward vmBy.screenHeight 1
        |> scrollHalf windowBefore 
        |> shouldEqual (windowAfter, Window.Event.ContentsUpdated windowAfter :> Message)

    [<Test>]
    member x.``down when exactly half a screen is showing should leave the last line showing``() =
        let dimensions = { Rows = 12<mRow>; Columns = 60<mColumn> }
        let windowBefore = { Window.defaultWindowView with Buffer = !buffer; Dimensions = dimensions }
        let windowAfter = { windowBefore with TopLineNumber = 6<mLine>; Buffer = ["f"] }

        Move.forward vmBy.screenHeight 1
        |> scrollHalf windowBefore 
        |> shouldEqual (windowAfter, Window.Event.ContentsUpdated windowAfter :> Message)
