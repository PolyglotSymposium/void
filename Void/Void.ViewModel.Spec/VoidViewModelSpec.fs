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
type ``Rendering text lines as drawing objects for a view size``() = 
    [<Test>]
    member x.``for one line, which fits on the screen in both dimensions, should place it at the origin``() =
        Render.linesAsDrawingObjects Sizing.defaultViewSize ["Just one line"]
        |> should equal [DrawingObject.Text {
            Text = "Just one line"
            UpperLeftCorner = originCell
            Color = Colors.defaultColorscheme.Foreground
        }]

[<TestFixture>]
type ``Rendering the command bar``() = 
    [<Test>]
    member x.``when it is hidden results in a background block``() =
        Render.commandBarAsDrawingObjects CommandBarView.Hidden 78us { Row = 24us; Column = 0us }
        |> should equal [DrawingObject.Block {
            Area =
                {
                    UpperLeftCorner = { Row = 24us; Column = 0us }
                    Dimensions = { Rows = 1us; Columns = 78us }
                }
            Color = Colors.defaultColorscheme.Background
        }]

    [<Test>]
    member x.``when it is shown but empty results in a prompt symbol``() =
        let drawings = Render.commandBarAsDrawingObjects (CommandBarView.Visible "") 80us { Row = 25us; Column = 0us }
        drawings.Length |> should equal 2
        drawings.[1] |> should equal (DrawingObject.Text {
            Text = ";"
            UpperLeftCorner = { Row = 25us; Column = 0us }
            Color = Colors.defaultColorscheme.DimForeground
        })

    [<Test>]
    member x.``when it is shown and has text results in a prompt symbol and text``() =
        let drawings = Render.commandBarAsDrawingObjects (CommandBarView.Visible "quit") 80us { Row = 25us; Column = 0us }
        drawings.Length |> should equal 3
        drawings.[2] |> should equal (DrawingObject.Text {
            Text = "quit"
            UpperLeftCorner = { Row = 25us; Column = 1us }
            Color = Colors.defaultColorscheme.Foreground
        })