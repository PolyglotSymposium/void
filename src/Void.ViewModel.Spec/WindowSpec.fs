namespace Void.ViewModel.Spec

open Void.ViewModel
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
