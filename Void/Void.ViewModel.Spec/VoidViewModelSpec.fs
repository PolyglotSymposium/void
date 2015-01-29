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
            FontMetrics = { LineHeight = 10us; CharWidth = 5us }
        } |> should equal { Height = 250us; Width = 400us }

[<TestFixture>]
type ``Rendering text lines as view objects for a view size``() = 
    [<Test>]
    member x.``for one line, which fits on the screen in both dimensions, should place it at the origin``() =
        Render.linesAsViewObjects Sizing.defaultViewSize ["Just one line"]
        |> should equal [ViewObject.Text {
            Text = "Just one line"
            UpperLeftCorner = origin
            Background = Colors.defaultColorscheme.Background
            Foreground = Colors.defaultColorscheme.Foreground
        }]