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

    [<SetUp>]
    member x.``Set up``() =
        requestSenderStub.reset()

    [<Test>]
    member x.``up when we are already at the top of the file should do nothing``() =
        []
        |> should equal []

    [<Test>]
    member x.``up one line when the top line is two should work``() =
        []
        |> should equal []

    [<Test>]
    member x.``up three lines when the top line is four should go to the top of the file``() =
        []
        |> should equal []

    [<Test>]
    member x.``up three lines when the top line is three should go to the top of the file``() =
        []
        |> should equal []

    [<Test>]
    member x.``down when the buffer is empty should do nothing``() =
        []
        |> should equal []

    [<Test>]
    member x.``down when only the last line of the buffer is showing should do nothing``() =
        []
        |> should equal []

    [<Test>]
    member x.``down one line when there is another line not shown``() =
        []
        |> should equal []

    [<Test>]
    member x.``down one line when there is nothing more when the top line shown is not the bottom line of the file``() =
        []
        |> should equal []

    [<Test>]
    member x.``down multiple lines``() =
        let windowBefore = { Window.defaultWindowView with Buffer = ["a"; "b"; "c"; "d"; "e"; "f"] }
        let windowAfter = { windowBefore with TopLineNumber = 4<mLine>; Buffer = ["d"; "e"; "f"] }
        ({
            FirstLineNumber = 4<mLine>
            RequestedContents = ["d"; "e"; "f"]
        } : GetWindowContentsResponse) |> requestSenderStub.registerResponse

        Move.Forward 3<mLine>
        |> VMCommand.Scroll
        |> Window.handleVMCommand requestSenderStub windowBefore 
        |> shouldEqual (windowAfter, Window.Event.ContentsUpdated windowAfter :> Message)
