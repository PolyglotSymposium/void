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
        RenderWindows.textLinesAsDrawingObjects ["Just one line"]
        |> should equal [DrawingObject.Text {
            Text = "Just one line"
            UpperLeftCorner = PointGrid.originPoint
            Color = Colors.defaultColorscheme.Foreground
        }]

[<TestFixture>]
type ``Rendering buffers``() = 
    let windowArea = {
        UpperLeftCell = originCell
        Dimensions = { Rows = 25; Columns = 80 }
    }

    let render = RenderWindows.contentsAsDrawingObjects windowArea

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
        let drawingObjects =  render []
        drawingObjects.Length |> should equal 25
        drawingObjects.[0] |> shouldBeBackgroundBlock
        drawingObjects.Tail |> shouldAllBeTildes

    [<Test>]
    member x.``when the buffer has one line it renders that line and but otherwise is like an empty buffer``() =
        let drawingObjects = render ["only one line"]
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
        let drawingObjects = render ["line 1"; "line 2"]
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
        let drawingObjects = render (Enumerable.Repeat("line", 25) |> List.ofSeq)
        drawingObjects.Length |> should equal 26
        drawingObjects.[0] |> shouldBeBackgroundBlock
        drawingObjects |> Seq.mapi (fun i drawingObject ->
            drawingObject |> should equal [DrawingObject.Text {
                Text = "line"
                UpperLeftCorner = { Y = i+1; X = 0 }
                Color = Colors.defaultColorscheme.DimForeground
            }]) |> ignore
