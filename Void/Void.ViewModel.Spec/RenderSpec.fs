namespace Void.ViewModel.Spec

open Void.ViewModel
open Void.ViewModel.PixelGrid
open Void.ViewModel.CellGrid
open NUnit.Framework
open FsUnit

module Stubs =
    let cellToPoint cell = { X = cell.Column; Y = cell.Row }
    let dimensionsInPixels dimensions = { Height = dimensions.Rows; Width = dimensions.Columns }

[<TestFixture>]
type ``Rendering text lines as drawing objects for a view size``() = 
    [<Test>]
    member x.``for one line, which fits on the screen in both dimensions, should place it at the origin``() =
        Render.textLinesAsDrawingObjects Stubs.cellToPoint ["Just one line"]
        |> should equal [DrawingObject.Text {
            Text = "Just one line"
            UpperLeftCorner = originPoint
            Color = Colors.defaultColorscheme.Foreground
        }]

[<TestFixture>]
type ``Rendering the command bar``() = 
    [<Test>]
    member x.``when it is hidden results in a background block``() =
        Render.commandBarAsDrawingObjects Stubs.cellToPoint CommandBarView.Hidden 78 { Row = 24; Column = 0 }
        |> should equal [DrawingObject.Block {
            Area =
                {
                    UpperLeftCorner = { Y = 24; X = 0 }
                    Dimensions = { Height = 1; Width = 78 }
                }
            Color = Colors.defaultColorscheme.Background
        }]

    [<Test>]
    member x.``when it is shown but empty results in a prompt symbol``() =
        let drawings = Render.commandBarAsDrawingObjects Stubs.cellToPoint (CommandBarView.Visible "") 80 { Row = 25; Column = 0 }
        drawings.Length |> should equal 2
        drawings.[1] |> should equal (DrawingObject.Text {
            Text = ";"
            UpperLeftCorner = { Y = 25; X = 0 }
            Color = Colors.defaultColorscheme.DimForeground
        })

    [<Test>]
    member x.``when it is shown and has text results in a prompt symbol and text``() =
        let drawings = Render.commandBarAsDrawingObjects Stubs.cellToPoint (CommandBarView.Visible "quit") 80 { Row = 25; Column = 0 }
        drawings.Length |> should equal 3
        drawings.[2] |> should equal (DrawingObject.Text {
            Text = "quit"
            UpperLeftCorner = { Y = 25; X = 1 }
            Color = Colors.defaultColorscheme.Foreground
        })

[<TestFixture>]
type ``Rendering output messages``() = 
    [<Test>]
    member x.``when there are none results in no drawing objects``() =
        Render.outputMessagesAsDrawingObjects Stubs.cellToPoint [] 78 { Row = 24; Column = 0 }
        |> should equal []

    [<Test>]
    member x.``when there is one error renders it in error colors``() =
        Render.outputMessagesAsDrawingObjects Stubs.cellToPoint [OutputMessageView.Error "Bad!"] 78 { Row = 24; Column = 0 }
        |> should equal [DrawingObject.Text {
            Text = "Bad!"
            UpperLeftCorner = { Y = 24; X = 0 }
            Color = Colors.defaultColorscheme.Error
        }]

    [<Test>]
    member x.``when there is one message renders it in regular colors``() =
        Render.outputMessagesAsDrawingObjects Stubs.cellToPoint [OutputMessageView.Text "Good"] 80 { Row = 25; Column = 0 }
        |> should equal [DrawingObject.Text {
            Text = "Good"
            UpperLeftCorner = { Y = 25; X = 0 }
            Color = Colors.defaultColorscheme.Foreground
        }]

    // TODO multiple lines

[<TestFixture>]
type ``Rendering buffers``() = 
    let windowArea = {
        UpperLeftCorner = originCell
        Dimensions = { Rows = 25; Columns = 80 }
    }

    [<Test>]
    member x.``when the buffer is empty it renders as an empty background-colored area``() =
        Render.bufferAsDrawingObjects Stubs.cellToPoint Stubs.dimensionsInPixels windowArea { Contents = [] }
        |> should equal [DrawingObject.Block {
            Area = { UpperLeftCorner = originPoint; Dimensions = { Height = 25; Width = 80 } }
            Color = Colors.defaultColorscheme.Background
        }]
