namespace Void.ViewModel.Spec

open Void.ViewModel
open Void.Core
open Void.Core.CellGrid
open System
open System.Linq
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Constructing a buffer view model from a sequence of text lines``() = 
    let asViewModelBuffer = Window.bufferFrom { Rows = 25; Columns = 80 }

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
type ``Scrolling``() = 
    let requestSenderStub = CannedResponseRequestSender()

    let shouldEqual expected actual =
        printfn "Expected: %A" expected
        printfn "Actual: %A" actual
        should equal expected actual

    let scroll window movement =
        VMCommand.Scroll movement
        |> Window.handleVMCommand requestSenderStub window

    let respondWith firstLineNumber contents =
        ({
            FirstLineNumber = firstLineNumber
            RequestedContents = contents
        } : GetWindowContentsResponse) |> requestSenderStub.registerResponse

    [<SetUp>]
    member x.``Set up``() =
        requestSenderStub.reset()

    [<Test>]
    member x.``up when we are already at the top of the file should do nothing``() =
        let windowBefore = { Window.defaultWindowView with Buffer = ["a"; "b"; "c"; "d"; "e"; "f"] }
        respondWith 4<mLine> ["d"; "e"; "f"]

        Move.Backward 3<mLine>
        |> scroll windowBefore 
        |> shouldEqual (windowBefore, noMessage)

    [<Test>]
    member x.``up one line when the top line is two should work``() =
        let windowBefore = { Window.defaultWindowView with Buffer = ["b"; "c"; "d"; "e"; "f"]; TopLineNumber = 2<mLine> }
        let windowAfter = { windowBefore with TopLineNumber = 1<mLine>; Buffer = ["a"; "b"; "c"; "d"; "e"; "f"] }
        respondWith 1<mLine> ["a"; "b"; "c"; "d"; "e"; "f"]

        Move.Backward 1<mLine>
        |> scroll windowBefore 
        |> shouldEqual (windowAfter, Window.Event.ContentsUpdated windowAfter :> Message)

    [<Test>]
    member x.``up three lines when the top line is four should go to the top of the file``() =
        let windowBefore = { Window.defaultWindowView with Buffer = ["d"; "e"; "f"]; TopLineNumber = 4<mLine> }
        let windowAfter = { windowBefore with TopLineNumber = 1<mLine>; Buffer = ["a"; "b"; "c"; "d"; "e"; "f"] }
        respondWith 1<mLine> ["a"; "b"; "c"; "d"; "e"; "f"]

        Move.Backward 3<mLine>
        |> scroll windowBefore 
        |> shouldEqual (windowAfter, Window.Event.ContentsUpdated windowAfter :> Message)

    [<Test>]
    member x.``up four lines when the top line is three should go to the top of the file``() =
        let windowBefore = { Window.defaultWindowView with Buffer = ["d"; "e"; "f"]; TopLineNumber = 3<mLine> }
        let windowAfter = { windowBefore with TopLineNumber = 1<mLine>; Buffer = ["a"; "b"; "c"; "d"; "e"; "f"] }
        respondWith 1<mLine> ["a"; "b"; "c"; "d"; "e"; "f"]

        Move.Backward 4<mLine>
        |> scroll windowBefore 
        |> shouldEqual (windowAfter, Window.Event.ContentsUpdated windowAfter :> Message)

    [<Test>]
    member x.``up when the buffer is empty should do nothing``() =
        let windowBefore = Window.defaultWindowView
        respondWith 1<mLine> []

        Move.Backward 1<mLine>
        |> scroll windowBefore 
        |> shouldEqual (windowBefore, noMessage)

    [<Test>]
    member x.``down when the buffer is empty should do nothing``() =
        let windowBefore = Window.defaultWindowView
        respondWith 1<mLine> []

        Move.Forward 1<mLine>
        |> scroll windowBefore 
        |> shouldEqual (windowBefore, noMessage)

    [<Test>]
    member x.``down when only the last line of the buffer is showing should do nothing``() =
        let windowBefore = { Window.defaultWindowView with TopLineNumber = 6<mLine>; Buffer = ["f"] }
        respondWith 7<mLine> []

        Move.Forward 1<mLine>
        |> scroll windowBefore 
        |> shouldEqual (windowBefore, noMessage)

    [<Test>]
    member x.``down multiple lines from the top``() =
        let windowBefore = { Window.defaultWindowView with Buffer = ["a"; "b"; "c"; "d"; "e"; "f"] }
        let windowAfter = { windowBefore with TopLineNumber = 4<mLine>; Buffer = ["d"; "e"; "f"] }
        respondWith 4<mLine> ["d"; "e"; "f"]

        Move.Forward 3<mLine>
        |> scroll windowBefore 
        |> shouldEqual (windowAfter, Window.Event.ContentsUpdated windowAfter :> Message)
