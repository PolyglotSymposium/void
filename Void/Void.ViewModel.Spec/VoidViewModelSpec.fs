namespace Void.ViewModel.Spec

open Void.ViewModel
open Void.ViewModel.PixelGrid
open Void.ViewModel.CellGrid
open System
open System.Linq
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``When converting from cells to pixels``() = 
    let fontMetrics = { LineHeight = 10; CharWidth = 5 }
    let pointAtUpperLeftOf = Sizing.pointAtUpperLeftOfCell fontMetrics
    let cellDimenionsToPixels = Sizing.cellDimensionsToPixels fontMetrics

    [<Test>]
    member x.``the upper left corner of the origin cell is the origin point``() =
        pointAtUpperLeftOf originCell |> should equal originPoint
    [<Test>]
    member x.``the upper left corner of a cell away from the origin should be computed with font metrics``() =
        pointAtUpperLeftOf { Row = 1; Column = 1 } |> should equal { X = 5; Y = 10 }

    [<Test>]
    member x.``zero-sized cell dimensions should be zero-sized in pixels as well``() =
        cellDimenionsToPixels { Rows = 0; Columns = 0 } |> should equal { Height = 0; Width = 0 }
    [<Test>]
    member x.``cell dimenions should be scaled by font metrics to produce pixel dimensions``() =
        cellDimenionsToPixels { Rows = 25; Columns = 80 } |> should equal { Height = 250; Width = 400 }

[<TestFixture>]
type ``Constructing a buffer view model from a sequence of text lines``() = 
    let asViewModelBuffer = ViewModel.bufferFrom { Rows = 25; Columns = 80 }

    [<Test>]
    member x.``should create an empty buffer view model from an empty buffer``() =
        asViewModelBuffer Seq.empty
        |> should equal { Contents = [] }

    [<Test>]
    member x.``for one line, shorter than the window width, should create a buffer with one line``() =
        seq { yield "line 1" }
        |> asViewModelBuffer 
        |> should equal { Contents = ["line 1"] }

    [<Test>]
    member x.``for one line, with length equal to the window width, should create a buffer with one line``() =
        seq { yield String('X', 80) }
        |> asViewModelBuffer 
        |> should equal { Contents = [String('X', 80)] }

    [<Test>]
    member x.``for one line, longer than the window width, should truncate the visible part of the line``() =
        // TODO this is the behavior when Vim's wrap option is set to nowrap
        seq { yield String('x', 81) }
        |> asViewModelBuffer 
        |> should equal { Contents = [String('x', 80)] }

    [<Test>]
    member x.``for a buffer which has more lines than the window has height, should create a buffer view model to fill the window size``() =
        Enumerable.Repeat("line", 26)
        |> asViewModelBuffer 
        |> should equal { Contents = Seq.toList <| Enumerable.Repeat("line", 25) }
