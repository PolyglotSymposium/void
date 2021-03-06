﻿namespace Void.ViewModel.Spec

open Void.ViewModel
open Void.Core
open Void.Core.CellGrid
open System
open System.Linq
open NUnit.Framework
open FsUnit

[<AutoOpen>]
module WindowSpecUtil = 
    let buffer25lines =
        Seq.initInfinite (sprintf "%i")
        |> Seq.take 25
        |> Seq.toList

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
        |> should equal (windowBefore, noMessage)

    [<Test>]
    member x.``up one line when the top line is two should work``() =
        let windowBefore = { Window.defaultWindowView with Buffer = ["b"; "c"; "d"; "e"; "f"]; TopLineNumber = 2<mLine> }
        let windowAfter = { windowBefore with TopLineNumber = 1<mLine>; Buffer = !buffer }

        Move.backward By.line 1
        |> scroll windowBefore 
        |> should equal (windowAfter, Window.Event.ContentsScrolled windowAfter :> Message)

    [<Test>]
    member x.``up three lines when the top line is four should go to the top of the file``() =
        let windowBefore = { Window.defaultWindowView with Buffer = ["d"; "e"; "f"]; TopLineNumber = 4<mLine> }
        let windowAfter = { windowBefore with TopLineNumber = 1<mLine>; Buffer = !buffer }

        Move.backward By.line 3
        |> scroll windowBefore 
        |> should equal (windowAfter, Window.Event.ContentsScrolled windowAfter :> Message)

    [<Test>]
    member x.``up four lines when the top line is three should go to the top of the file``() =
        let windowBefore = { Window.defaultWindowView with Buffer = ["d"; "e"; "f"]; TopLineNumber = 3<mLine> }
        let windowAfter = { windowBefore with TopLineNumber = 1<mLine>; Buffer = !buffer }

        Move.backward By.line 4
        |> scroll windowBefore 
        |> should equal (windowAfter, Window.Event.ContentsScrolled windowAfter :> Message)

    [<Test>]
    member x.``up when the buffer is empty should do nothing``() =
        buffer := []
        let windowBefore = Window.defaultWindowView

        Move.backward By.line 1
        |> scroll windowBefore 
        |> should equal (windowBefore, noMessage)

    [<Test>]
    member x.``down when the buffer is empty should do nothing``() =
        buffer := []
        let windowBefore = Window.defaultWindowView

        Move.forward By.line 1
        |> scroll windowBefore 
        |> should equal (windowBefore, noMessage)

    [<Test>]
    member x.``down when only the last line of the buffer is showing should do nothing``() =
        let windowBefore = { Window.defaultWindowView with TopLineNumber = 6<mLine>; Buffer = ["f"] }

        Move.forward By.line 1
        |> scroll windowBefore 
        |> should equal (windowBefore, noMessage)

    [<Test>]
    member x.``down should move the window-relative cursor if necessary to keep it from being on an non-existent line``() =
        buffer := buffer25lines
        let windowBefore = { Window.defaultWindowView with TopLineNumber = 1<mLine>; Buffer = !buffer }
        let windowBefore = Window.setCursorPosition windowBefore { Row = 24<mRow>; Column = 0<mColumn> }
        let windowAfter = { Window.defaultWindowView with TopLineNumber = 11<mLine>; Buffer = Seq.toList <| Seq.skip 10 buffer25lines }
        let windowAfter = Window.setCursorPosition windowAfter { Row = 14<mRow>; Column = 0<mColumn> }

        Move.forward By.line 10
        |> scroll windowBefore 
        |> should equal (windowAfter, Window.Event.ContentsScrolled windowAfter :> Message)

    [<Test>]
    member x.``down multiple lines from the top``() =
        let windowBefore = { Window.defaultWindowView with Buffer = !buffer }
        let windowAfter = { windowBefore with TopLineNumber = 4<mLine>; Buffer = ["d"; "e"; "f"] }

        Move.forward By.line 3
        |> scroll windowBefore 
        |> should equal (windowAfter, Window.Event.ContentsScrolled windowAfter :> Message)

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
        |> should equal (windowBefore, noMessage)

    [<Test>]
    member x.``up half a screen height when the top line is two should go to the top of the file``() =
        let windowBefore = { Window.defaultWindowView with Buffer = ["b"; "c"; "d"; "e"; "f"]; TopLineNumber = 2<mLine> }
        let windowAfter = { windowBefore with TopLineNumber = 1<mLine>; Buffer = !buffer }

        Move.backward vmBy.screenHeight 1
        |> scrollHalf windowBefore 
        |> should equal (windowAfter, Window.Event.ContentsScrolled windowAfter :> Message)

    [<Test>]
    member x.``up when the buffer is empty should do nothing``() =
        buffer := []
        let windowBefore = Window.defaultWindowView

        Move.backward vmBy.screenHeight 1
        |> scrollHalf windowBefore 
        |> should equal (windowBefore, noMessage)

    [<Test>]
    member x.``down when the buffer is empty should do nothing``() =
        buffer := []
        let windowBefore = Window.defaultWindowView

        Move.forward vmBy.screenHeight 1
        |> scrollHalf windowBefore 
        |> should equal (windowBefore, noMessage)

    [<Test>]
    member x.``down when only the last line of the buffer is showing should do nothing``() =
        let windowBefore = { Window.defaultWindowView with TopLineNumber = 6<mLine>; Buffer = ["f"] }

        Move.forward vmBy.screenHeight 1
        |> scrollHalf windowBefore 
        |> should equal (windowBefore, noMessage)

    [<Test>]
    member x.``down when less than half a screen is showing should leave last line showing``() =
        let windowBefore = { Window.defaultWindowView with Buffer = !buffer }
        let windowAfter = { windowBefore with TopLineNumber = 6<mLine>; Buffer = ["f"] }

        Move.forward vmBy.screenHeight 1
        |> scrollHalf windowBefore 
        |> should equal (windowAfter, Window.Event.ContentsScrolled windowAfter :> Message)

    [<Test>]
    member x.``down when exactly half a screen is showing should leave the last line showing``() =
        let dimensions = { Rows = 12<mRow>; Columns = 60<mColumn> }
        let windowBefore = { Window.defaultWindowView with Buffer = !buffer; Dimensions = dimensions }
        let windowAfter = { windowBefore with TopLineNumber = 6<mLine>; Buffer = ["f"] }

        Move.forward vmBy.screenHeight 1
        |> scrollHalf windowBefore 
        |> should equal (windowAfter, Window.Event.ContentsScrolled windowAfter :> Message)

[<TestFixture>]
type ``Moving the cursor``() = 
    [<Test>]
    member x.``When the cursor is moved in the buffer, it is moved in the window``() =
        let targetCell = below originCell 1<mRow>
        let windowBefore = { Window.defaultWindowView with Buffer = ["a"; "b"] }
        let windowAfter = Window.setCursorPosition windowBefore targetCell

        { BufferId = 1; Message = BufferEvent.CursorMoved(originCell, targetCell) }
        |> Window.handleBufferEvent windowBefore
        |> should equal (windowAfter, Window.Event.CursorMoved(originCell, targetCell, windowAfter) :> Message)

    [<Test>]
    member x.``The cursor in the window is tracked relative to the window, not the buffer``() =
        let targetCell = below originCell 1<mRow>
        let windowBefore = { Window.defaultWindowView with Buffer = buffer25lines; TopLineNumber = 10<mLine> }
        let windowAfter = Window.setCursorPosition windowBefore targetCell

        { BufferId = 1; Message = BufferEvent.CursorMoved(below originCell 9<mRow>, below targetCell 9<mRow>) }
        |> Window.handleBufferEvent windowBefore
        |> should equal (windowAfter, Window.Event.CursorMoved(originCell, targetCell, windowAfter) :> Message)

    [<Test>]
    member x.``When the buffer cursor moves below what is visible in the window, the buffer is scrolled down``() =
        let startCell = below originCell 24<mRow>
        let window = { Window.setCursorPosition Window.defaultWindowView startCell with Buffer = buffer25lines }

        { BufferId = 1; Message = BufferEvent.CursorMoved(startCell, below startCell 5<mRow>) }
        |> Window.handleBufferEvent window
        |> should equal (window, VMCommand.Scroll (Move.forward By.line 5) :> Message)

    [<Test>]
    member x.``When the buffer cursor moves above what is visible in the window, the buffer is scrolled up``() =
        let window = { Window.defaultWindowView with Buffer = ["z"]; TopLineNumber = 30<mLine> }

        { BufferId = 1; Message = BufferEvent.CursorMoved(below originCell 29<mRow>, below originCell 26<mRow>) }
        |> Window.handleBufferEvent window
        |> should equal (window, VMCommand.Scroll (Move.backward By.line 3) :> Message)
