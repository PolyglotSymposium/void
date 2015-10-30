namespace Void.ViewModel.Spec

open Void.ViewModel
open Void.Core
open Void.Core.CellGrid
open System.Linq
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Rendering user notifications``() = 
    let render = RenderNotificationBar.asDrawingObjects
    [<Test>]
    member x.``when there are none results in no drawing objects``() =
        render 78 { Row = 24<mRow>; Column = 0<mColumn> } [] 
        |> should equal []

    [<Test>]
    member x.``when there is one error renders it in error colors``() =
        render 78 { Row = 24<mRow>; Column = 0<mColumn> } [UserNotificationView.Error "Bad!"]
        |> should equal [DrawingObject.Text {
            Text = "Bad!"
            UpperLeftCorner = { Y = 24; X = 0 }
            Color = Colors.defaultColorscheme.Error
        }]

    [<Test>]
    member x.``when there is one notification renders it in regular colors``() =
        render 80 { Row = 25<mRow>; Column = 0<mColumn> } [UserNotificationView.Text "Good"]
        |> should equal [DrawingObject.Text {
            Text = "Good"
            UpperLeftCorner = { Y = 25; X = 0 }
            Color = Colors.defaultColorscheme.Foreground
        }]

    // TODO multiple lines
