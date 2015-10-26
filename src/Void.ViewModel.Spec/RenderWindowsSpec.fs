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
    let render = RenderWindows.contentsAsDrawingObjects { Rows = 25<mRow>; Columns = 80<mColumn> }

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
    member x.``when the buffer is empty and unfocused it renders as a background-colored area with muted tildes on each line except the first``() =
        // TODO when you open an empty buffer in Vim, why is there no tilde in the first line?
        let drawingObjects =  render []
        drawingObjects.Length |> should equal 25
        drawingObjects.[0] |> shouldBeBackgroundBlock
        drawingObjects.Tail |> shouldAllBeTildes

    [<Test>]
    member x.``when the buffer is empty and focused it renders the normal-mode cursor at the origin cell``() =
        let drawingObjects = RenderWindows.windowAsDrawingObjects Window.defaultWindowView
        drawingObjects.Length |> should equal 26
        drawingObjects.[25] |> should equal (DrawingObject.Block {
            Area = GridConvert.boxAroundOneCell originCell
            Color = Colors.defaultColorscheme.Foreground
        })

    [<Test>]
    member x.``when the buffer has one line it renders that line but otherwise is like an empty buffer``() =
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
    member x.``when the buffer has one line and is focused it renders the normal-mode cursor``() =
        let window = { Window.defaultWindowView with Buffer = ["foo"] }
        let drawingObjects = RenderWindows.windowAsDrawingObjects window
        drawingObjects.Length |> should equal 28
        drawingObjects.[26] |> should equal (DrawingObject.Block {
            Area = GridConvert.boxAroundOneCell originCell
            Color = Colors.defaultColorscheme.Foreground
        })
        drawingObjects.[27] |> should equal (DrawingObject.Text {
            Text = "f"
            UpperLeftCorner = PointGrid.originPoint
            Color = Colors.defaultColorscheme.Background
        })

    [<Test>]
    member x.``when the buffer has text and is focused and the cursor is not at the origin, it renders the normal-mode cursor``() =
        let cursorCell = { Row = 1<mRow>; Column = 1<mColumn> }
        let cursor = { Position = cursorCell; Appearance = Visible CursorStyle.Block }
        let window = { Window.defaultWindowView with Buffer = ["foo"; "bar"; "bez"]; Cursor = cursor }
        let drawingObjects = RenderWindows.windowAsDrawingObjects window
        drawingObjects.Length |> should equal 28
        drawingObjects.[26] |> should equal (DrawingObject.Block {
            Area = GridConvert.boxAroundOneCell cursorCell
            Color = Colors.defaultColorscheme.Foreground
        })
        drawingObjects.[27] |> should equal (DrawingObject.Text {
            Text = "a"
            UpperLeftCorner = GridConvert.upperLeftCornerOf cursorCell
            Color = Colors.defaultColorscheme.Background
        })

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

    [<Test>]
    member x.``when the cursor is moved in normal mode, it only renders the from and to cells again``() =
        let cursorCell = { Row = 1<mRow>; Column = 1<mColumn> }
        let cursor = { Position = cursorCell; Appearance = Visible CursorStyle.Block }
        let window = { Window.defaultWindowView with Buffer = ["foo"; "bar"; "bez"]; Cursor = cursor }
        let event = Window.Event.CursorMoved (originCell, cursorCell, window)
        match RenderWindows.handleWindowEvent event with
        | :? VMEvent as vmEvent ->
            match vmEvent with
            | VMEvent.MultipleViewPortionsRendered viewPortions ->
                Seq.length viewPortions |> should equal 2
                let block1, viewPortion1 = Seq.head viewPortions
                let block2, viewPortion2 = Seq.last viewPortions
                block1 |> should equal (GridConvert.boxAroundOneCell originCell)
                block2 |> should equal (GridConvert.boxAroundOneCell cursorCell)
                Seq.length viewPortion1 |> should equal 2
                Seq.length viewPortion2 |> should equal 2
                let drawingObject1 = Seq.head viewPortion1
                let drawingObject2 = Seq.last viewPortion1
                let drawingObject3 = Seq.head viewPortion2
                let drawingObject4 = Seq.last viewPortion2
                drawingObject1 |> should equal (DrawingObject.Block {
                    Area = block1
                    Color = Colors.defaultColorscheme.Background
                })
                drawingObject2 |> should equal (DrawingObject.Text {
                    Text = "f"
                    UpperLeftCorner = PointGrid.originPoint
                    Color = Colors.defaultColorscheme.Foreground
                })
                drawingObject3 |> should equal (DrawingObject.Block {
                    Area = block2
                    Color = Colors.defaultColorscheme.Foreground
                })
                drawingObject4 |> should equal (DrawingObject.Text {
                    Text = "a"
                    UpperLeftCorner = GridConvert.upperLeftCornerOf cursorCell
                    Color = Colors.defaultColorscheme.Background
                })
            | _ -> Assert.Fail("Expected MultipleViewPortionsRendered")
        | _ -> Assert.Fail("Expected VMEvent")
