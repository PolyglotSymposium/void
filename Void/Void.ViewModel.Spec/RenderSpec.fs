namespace Void.ViewModel.Spec

open Void.ViewModel
open Void.ViewModel.PixelGrid
open Void.ViewModel.CellGrid
open NUnit.Framework
open FsUnit

module Stubs =
    let convert = Sizing.Convert { LineHeight = 1; CharWidth = 1 }

[<TestFixture>]
type ``Rendering text lines as drawing objects for a view size``() = 
    [<Test>]
    member x.``for one line, which fits on the screen in both dimensions, should place it at the origin``() =
        Render.textLinesAsDrawingObjects Stubs.convert ["Just one line"]
        |> should equal [DrawingObject.Text {
            Text = "Just one line"
            UpperLeftCorner = originPoint
            Color = Colors.defaultColorscheme.Foreground
        }]

[<TestFixture>]
type ``Rendering the command bar``() = 
    [<Test>]
    member x.``when it is hidden results in a background block``() =
        Render.commandBarAsDrawingObjects Stubs.convert CommandBarView.Hidden 78 { Row = 24; Column = 0 }
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
        let drawings = Render.commandBarAsDrawingObjects Stubs.convert (CommandBarView.Visible "") 80 { Row = 25; Column = 0 }
        drawings.Length |> should equal 2
        drawings.[1] |> should equal (DrawingObject.Text {
            Text = ";"
            UpperLeftCorner = { Y = 25; X = 0 }
            Color = Colors.defaultColorscheme.DimForeground
        })

    [<Test>]
    member x.``when it is shown and has text results in a prompt symbol and text``() =
        let drawings = Render.commandBarAsDrawingObjects Stubs.convert (CommandBarView.Visible "quit") 80 { Row = 25; Column = 0 }
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
        Render.outputMessagesAsDrawingObjects Stubs.convert [] 78 { Row = 24; Column = 0 }
        |> should equal []

    [<Test>]
    member x.``when there is one error renders it in error colors``() =
        Render.outputMessagesAsDrawingObjects Stubs.convert [OutputMessageView.Error "Bad!"] 78 { Row = 24; Column = 0 }
        |> should equal [DrawingObject.Text {
            Text = "Bad!"
            UpperLeftCorner = { Y = 24; X = 0 }
            Color = Colors.defaultColorscheme.Error
        }]

    [<Test>]
    member x.``when there is one message renders it in regular colors``() =
        Render.outputMessagesAsDrawingObjects Stubs.convert [OutputMessageView.Text "Good"] 80 { Row = 25; Column = 0 }
        |> should equal [DrawingObject.Text {
            Text = "Good"
            UpperLeftCorner = { Y = 25; X = 0 }
            Color = Colors.defaultColorscheme.Foreground
        }]

    // TODO multiple lines

[<TestFixture>]
type ``Rendering buffers``() = 
    let windowArea = {
        UpperLeftCell = originCell
        Dimensions = { Rows = 25; Columns = 80 }
    }

    [<Test>]
    member x.``when the buffer is empty it renders as a background-colored area with muted tildes on each line except the first``() =
        // TODO when you open an empty buffer in Vim, why is there no tilde in the first line?
        let drawingObjects = Render.bufferAsDrawingObjects Stubs.convert windowArea { Contents = [] }
        drawingObjects.Length |> should equal 25
        drawingObjects.[0] |> should equal (DrawingObject.Block {
            Area = { UpperLeftCorner = originPoint; Dimensions = { Height = 25; Width = 80 } }
            Color = Colors.defaultColorscheme.Background
        })
        drawingObjects.Tail |> Seq.mapi (fun i drawingObject ->
            drawingObject |> should equal [DrawingObject.Text {
                Text = "~"
                UpperLeftCorner = Stubs.convert.cellToUpperLeftPoint { Row = i+1; Column = 0 }
                Color = Colors.defaultColorscheme.DimForeground
            }]
        )
