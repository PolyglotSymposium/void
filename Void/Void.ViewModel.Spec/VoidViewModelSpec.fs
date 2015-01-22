namespace Void.ViewModel.Spec

open Void.ViewModel
open Void.ViewModel.PixelGrid
open Void.ViewModel.CellGrid
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``The view size in pixels calculated from rows and columns and font metrics``() = 
    [<Test>]
    member x.``should be rounded ``() =
        Sizing.viewSizeInPixels {
            Dimensions = { Rows = 25us; Columns = 80us }
            FontMetrics = { LineHeight = 10.1; CharWidth = 5.51 }
        } |> should equal { Height = 253us; Width = 441us }
