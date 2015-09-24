namespace Void.ViewModel.Spec

open Void.ViewModel
open Void.Core
open Void.Core.CellGrid
open System.Linq
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Rendering the command bar``() = 
    [<Test>]
    member x.``when it is hidden results in a background block``() =
        let (_, drawings) = RenderCommandBar.asDrawingObjects CommandBar.hidden { Row = 24<mRow>; Column = 0<mColumn> }
        let drawings = List.ofSeq drawings
        drawings.Length |> should equal 1
        drawings.[0] |> should equal (DrawingObject.Block {
            Area =
                {
                    UpperLeftCorner = { Y = 24; X = 0 }
                    Dimensions = { Height = 1; Width = 80 }
                }
            Color = Colors.defaultColorscheme.Background
        })

    [<Test>]
    member x.``when it is shown but empty results in a prompt symbol``() =
        let (_, drawings) = RenderCommandBar.asDrawingObjects CommandBar.visibleButEmpty { Row = 25<mRow>; Column = 0<mColumn> }
        let drawings = List.ofSeq drawings
        drawings.Length |> should equal 2
        drawings.[1] |> should equal (DrawingObject.Text {
            Text = ";"
            UpperLeftCorner = { Y = 25; X = 0 }
            Color = Colors.defaultColorscheme.DimForeground
        })

    [<Test>]
    member x.``when it is shown but empty results in a prompt symbol (classic Vim mode)``() =
        let (_, drawings) = RenderCommandBar.asDrawingObjects { Prompt = Visible CommandBarPrompt.ClassicVim; WrappedLines = [""]; Width = 80 } { Row = 25<mRow>; Column = 0<mColumn> }
        let drawings = List.ofSeq drawings
        drawings.Length |> should equal 2
        drawings.[1] |> should equal (DrawingObject.Text {
            Text = ":"
            UpperLeftCorner = { Y = 25; X = 0 }
            Color = Colors.defaultColorscheme.DimForeground
        })

    [<Test>]
    member x.``when it is shown and has text results in a prompt symbol and text``() =
        let (_, drawings) = RenderCommandBar.asDrawingObjects { Prompt = Visible CommandBarPrompt.VoidDefault; WrappedLines = ["quit"]; Width = 80 } { Row = 25<mRow>; Column = 0<mColumn> }
        let drawings = List.ofSeq drawings
        drawings.Length |> should equal 3
        drawings.[2] |> should equal (DrawingObject.Text {
            Text = "quit"
            UpperLeftCorner = { Y = 25; X = 1 }
            Color = Colors.defaultColorscheme.Foreground
        })
