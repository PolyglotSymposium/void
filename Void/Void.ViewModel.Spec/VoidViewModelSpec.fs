namespace Void.ViewModel.Spec

open Void.ViewModel
open Void.ViewModel.PixelGrid
open Void.ViewModel.CellGrid
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``When converting from cells to pixels``() = 
    let fontMetrics = { LineHeight = 10us; CharWidth = 5us }
    let pointAtUpperLeftOf = Sizing.pointAtUpperLeftOfCell fontMetrics
    let cellDimenionsToPixels = Sizing.cellDimensionsToPixels fontMetrics

    [<Test>]
    member x.``the upper left corner of the origin cell is the origin point``() =
        pointAtUpperLeftOf originCell |> should equal originPoint
    [<Test>]
    member x.``the upper left corner of a cell away from the origin should be computed with font metrics``() =
        pointAtUpperLeftOf { Row = 1us; Column = 1us } |> should equal { X = 5us; Y = 10us }

    [<Test>]
    member x.``zero-sized cell dimensions should be zero-sized in pixels as well``() =
        cellDimenionsToPixels { Rows = 0us; Columns = 0us } |> should equal { Height = 0us; Width = 0us }
    [<Test>]
    member x.``cell dimenions should be scaled by font metrics to produce pixel dimensions``() =
        cellDimenionsToPixels { Rows = 25us; Columns = 80us } |> should equal { Height = 250us; Width = 400us }
