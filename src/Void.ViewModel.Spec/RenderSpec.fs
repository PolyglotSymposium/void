namespace Void.ViewModel.Spec

open Void.ViewModel
open Void.Core
open Void.Core.CellGrid
open System.Linq
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Rendering text lines as drawing objects for a view size``() = 
    [<Test>]
    member x.``for one line, which fits on the screen in both dimensions, should place it at the origin``() =
        Render.textLinesAsDrawingObjects ["Just one line"]
        |> should equal [DrawingObject.Text {
            Text = "Just one line"
            UpperLeftCorner = PointGrid.originPoint
            Color = Colors.defaultColorscheme.Foreground
        }]

[<TestFixture>]
type ``Rendering the command bar``() = 
    [<Test>]
    member x.``when it is hidden results in a background block``() =
        Render.commandBarAsDrawingObjects CommandBarView.Hidden 78 { Row = 24; Column = 0 }
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
        let drawings = Render.commandBarAsDrawingObjects (CommandBarView.Visible "") 80 { Row = 25; Column = 0 }
        drawings.Length |> should equal 2
        drawings.[1] |> should equal (DrawingObject.Text {
            Text = ";"
            UpperLeftCorner = { Y = 25; X = 0 }
            Color = Colors.defaultColorscheme.DimForeground
        })

    [<Test>]
    member x.``when it is shown and has text results in a prompt symbol and text``() =
        let drawings = Render.commandBarAsDrawingObjects (CommandBarView.Visible "quit") 80 { Row = 25; Column = 0 }
        drawings.Length |> should equal 3
        drawings.[2] |> should equal (DrawingObject.Text {
            Text = "quit"
            UpperLeftCorner = { Y = 25; X = 1 }
            Color = Colors.defaultColorscheme.Foreground
        })

[<TestFixture>]
type ``Rendering user notifications``() = 
    let render = Render.notificationsAsDrawingObjects
    [<Test>]
    member x.``when there are none results in no drawing objects``() =
        render 78 { Row = 24; Column = 0 } [] 
        |> should equal []

    [<Test>]
    member x.``when there is one error renders it in error colors``() =
        render 78 { Row = 24; Column = 0 } [UserNotificationView.Error "Bad!"]
        |> should equal [DrawingObject.Text {
            Text = "Bad!"
            UpperLeftCorner = { Y = 24; X = 0 }
            Color = Colors.defaultColorscheme.Error
        }]

    [<Test>]
    member x.``when there is one notification renders it in regular colors``() =
        render 80 { Row = 25; Column = 0 } [UserNotificationView.Text "Good"]
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

    let render = Render.bufferAsDrawingObjects windowArea

    let shouldAllBeTildes drawingObjects =
        drawingObjects |> Seq.mapi (fun i drawingObject ->
            drawingObject |> should equal [DrawingObject.Text {
                Text = "~"
                UpperLeftCorner = { Y = i+1; X = 0 }
                Color = Colors.defaultColorscheme.DimForeground
            }]) |> ignore

    let shouldBeBackgroundBlock drawingObject =
        drawingObject |> should equal (DrawingObject.Block {
            Area = { UpperLeftCorner = PointGrid.originPoint; Dimensions = { Height = 25; Width = 80 } }
            Color = Colors.defaultColorscheme.Background
        })

    [<Test>]
    member x.``when the buffer is empty it renders as a background-colored area with muted tildes on each line except the first``() =
        // TODO when you open an empty buffer in Vim, why is there no tilde in the first line?
        let drawingObjects =  render { LinesOfText = [] }
        drawingObjects.Length |> should equal 25
        drawingObjects.[0] |> shouldBeBackgroundBlock
        drawingObjects.Tail |> shouldAllBeTildes

    [<Test>]
    member x.``when the buffer has one line it renders that line and but otherwise is like an empty buffer``() =
        let drawingObjects = render { LinesOfText = ["only one line"] }
        drawingObjects.Length |> should equal 26
        drawingObjects.[0] |> shouldBeBackgroundBlock
        drawingObjects.[1] |> should equal (DrawingObject.Text {
            Text = "only one line"
            UpperLeftCorner = PointGrid.originPoint
            Color = Colors.defaultColorscheme.Foreground
        })
        drawingObjects |> Seq.skip 2 |> shouldAllBeTildes

    [<Test>]
    member x.``when the buffer has multple lines, but less than the rows that are available in the window``() =
        let drawingObjects = render { LinesOfText = ["line 1"; "line 2"] }
        drawingObjects.Length |> should equal 26
        drawingObjects.[0] |> shouldBeBackgroundBlock
        drawingObjects.[1] |> should equal (DrawingObject.Text {
            Text = "line 1"
            UpperLeftCorner = PointGrid.originPoint
            Color = Colors.defaultColorscheme.Foreground
        })
        drawingObjects.[2] |> should equal (DrawingObject.Text {
            Text = "line 2"
            UpperLeftCorner = { X = 0; Y = 1 }
            Color = Colors.defaultColorscheme.Foreground
        })
        drawingObjects |> Seq.skip 3 |> shouldAllBeTildes

    [<Test>]
    member x.``when the buffer has as many lines as the rows in the window, no tildes show``() =
        // There should never be more because of the way that the buffer view model gets constructed
        let drawingObjects = render { LinesOfText = Enumerable.Repeat("line", 25) |> List.ofSeq }
        drawingObjects.Length |> should equal 26
        drawingObjects.[0] |> shouldBeBackgroundBlock
        drawingObjects |> Seq.mapi (fun i drawingObject ->
            drawingObject |> should equal [DrawingObject.Text {
                Text = "line"
                UpperLeftCorner = { Y = i+1; X = 0 }
                Color = Colors.defaultColorscheme.DimForeground
            }]) |> ignore
