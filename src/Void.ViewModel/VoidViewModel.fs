namespace Void.ViewModel

open Void.Core

module PixelGrid =
    type FontMetrics = {
        LineHeight : int
        CharWidth : int
    }
    type Point = {
        X : int
        Y : int
    }
    type Dimensions = {
        Height : int
        Width : int
    }
    type Block = {
        UpperLeftCorner : Point
        Dimensions : Dimensions
    }

    let originPoint = { X = 0; Y = 0 }

module Sizing =
    open System
    open PixelGrid
    open Void.Core.CellGrid

    let defaultViewSize = { Rows = 26; Columns = 80 }

    let defaultViewArea = { UpperLeftCell = originCell; Dimensions = defaultViewSize }

    type Convert(_fontMetrics : FontMetrics) =
        member this.cellToUpperLeftPoint cell =
            {
                X = cell.Column * _fontMetrics.CharWidth
                Y = cell.Row * _fontMetrics.LineHeight
            }
        member this.cellDimensionsToPixels dimensions =
            {
                Height = _fontMetrics.LineHeight * dimensions.Rows
                Width = _fontMetrics.CharWidth * dimensions.Columns
            }
        member this.cellBlockToPixels block =
            {
                UpperLeftCorner = this.cellToUpperLeftPoint block.UpperLeftCell
                Dimensions = this.cellDimensionsToPixels block.Dimensions
            }

[<RequireQualifiedAccess>]
type CursorView =
    | Block of CellGrid.Cell
    | IBeam of PixelGrid.Point
    | Hidden

[<RequireQualifiedAccess>]
type StatusLineView = // TODO much yet to be done here
    | Unfocused
    | Focused

type BufferView = {
    Contents: string list // TODO this is naive obviously
}

type WindowView = {
    StatusLine : StatusLineView
    Area : CellGrid.Block
    Buffer : BufferView
    Cursor : CursorView
}

(* "Command line" is too equivocal. I mean the ; (or : in Vim) bar at the
 * bottom of the screen *)
[<RequireQualifiedAccess>]
type CommandBarView =
    | Hidden
    | Visible of string

[<RequireQualifiedAccess>]
type TabNameView =
    | Unfocused of string
    | Focused of string

[<RequireQualifiedAccess>]
type UserNotificationView =
    | Text of string
    | Error of string

type MainViewModel = {
    Size : CellGrid.Dimensions
    TabBar : TabNameView list
    VisibleWindows : WindowView list
    CommandBar : CommandBarView // for command mode
    Notifications : UserNotificationView list
}

module ViewModel =
    open Void.Util
    open Void.Core.CellGrid

    let defaultTitle = "Void - A text editor in the spirit of Vim"
    let defaultFontSize = 9uy
    let defaultBuffer = { Contents = [] }

    let defaultWindowView containingArea =
        {
            StatusLine = StatusLineView.Focused
            Buffer = defaultBuffer
            Area = lessRowsBelow 1 containingArea
            Cursor = CursorView.Block originCell
        }

    let defaultViewModel =
        {
            Size = Sizing.defaultViewSize
            TabBar = []
            VisibleWindows = [defaultWindowView Sizing.defaultViewArea]
            CommandBar = CommandBarView.Hidden
            Notifications = []
        }

    let bufferFrom (windowSize : Dimensions) lines =
        let truncateToWindowWidth = StringUtil.noLongerThan windowSize.Columns
        {
            Contents = lines
            |> SeqUtil.notMoreThan windowSize.Rows
            |> Seq.map truncateToWindowWidth
            |> Seq.toList
        }

    let toScreenBuffer windowSize buffer =
        Editor.readLines buffer 1
        |> bufferFrom windowSize

    let private loadBufferIntoWindow buffer window =
        { window with Buffer = toScreenBuffer window.Area.Dimensions buffer }

    let loadBuffer buffer view =
        { view with VisibleWindows = [loadBufferIntoWindow buffer view.VisibleWindows.[0]] }

    let wholeArea view =
        {
            UpperLeftCell = originCell
            Dimensions = view.Size
        }

    let toScreenNotification notification =
        match notification with
        | UserNotification.Output notificationText -> UserNotificationView.Text notificationText
        | UserNotification.Error error -> UserNotificationView.Error <| Errors.textOf error

    let addNotification viewModel notification =
        { viewModel with Notifications = notification :: viewModel.Notifications }
