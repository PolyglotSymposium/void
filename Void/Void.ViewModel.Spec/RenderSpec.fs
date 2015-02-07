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
        Render.commandBarAsDrawingObjects Stubs.cellToPoint CommandBarView.Hidden 78us { Row = 24us; Column = 0us }
        |> should equal [DrawingObject.Block {
            Area =
                {
                    UpperLeftCorner = { Y = 24us; X = 0us }
                    Dimensions = { Height = 1us; Width = 78us }
                }
            Color = Colors.defaultColorscheme.Background
        }]

    [<Test>]
    member x.``when it is shown but empty results in a prompt symbol``() =
        let drawings = Render.commandBarAsDrawingObjects Stubs.cellToPoint (CommandBarView.Visible "") 80us { Row = 25us; Column = 0us }
        drawings.Length |> should equal 2
        drawings.[1] |> should equal (DrawingObject.Text {
            Text = ";"
            UpperLeftCorner = { Y = 25us; X = 0us }
            Color = Colors.defaultColorscheme.DimForeground
        })

    [<Test>]
    member x.``when it is shown and has text results in a prompt symbol and text``() =
        let drawings = Render.commandBarAsDrawingObjects Stubs.cellToPoint (CommandBarView.Visible "quit") 80us { Row = 25us; Column = 0us }
        drawings.Length |> should equal 3
        drawings.[2] |> should equal (DrawingObject.Text {
            Text = "quit"
            UpperLeftCorner = { Y = 25us; X = 1us }
            Color = Colors.defaultColorscheme.Foreground
        })

[<TestFixture>]
type ``Rendering output messages``() = 
    [<Test>]
    member x.``when there are none results in no drawing objects``() =
        Render.outputMessagesAsDrawingObjects Stubs.cellToPoint [] 78us { Row = 24us; Column = 0us }
        |> should equal []

    [<Test>]
    member x.``when there is one error renders it in error colors``() =
        Render.outputMessagesAsDrawingObjects Stubs.cellToPoint [OutputMessageView.Error "Bad!"] 78us { Row = 24us; Column = 0us }
        |> should equal [DrawingObject.Text {
            Text = "Bad!"
            UpperLeftCorner = { Y = 24us; X = 0us }
            Color = Colors.defaultColorscheme.Error
        }]

    [<Test>]
    member x.``when there is one message renders it in regular colors``() =
        Render.outputMessagesAsDrawingObjects Stubs.cellToPoint [OutputMessageView.Text "Good"] 80us { Row = 25us; Column = 0us }
        |> should equal [DrawingObject.Text {
            Text = "Good"
            UpperLeftCorner = { Y = 25us; X = 0us }
            Color = Colors.defaultColorscheme.Foreground
        }]

    // TODO multiple lines