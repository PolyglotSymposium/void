namespace Void.ViewModel

module CellGrid =
    type Cell = {
        Row : int
        Column : int
    }
    type Dimensions = {
        Rows : int
        Columns : int
    }
    type Block = {
        UpperLeftCell : Cell
        Dimensions : Dimensions
    }
    let originCell = { Row = 0; Column = 0 }

    let rightOf cell count =
        { Row = cell.Row; Column = cell.Column + count }

    let below cell count =
        { Row = cell.Row + count; Column = cell.Column }

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
    open CellGrid

    let defaultViewSize = { Rows = 26; Columns = 80 }

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

[<RequireQualifiedAccess>]
type StatusLineView = // TODO much yet to be done here
    | Unfocused
    | Focused

type BufferView = {
    Contents: string list // TODO this is naive obviously
}

type UnfocusedWindowView = {
    StatusLine : StatusLineView
    Area : PixelGrid.Block
    Buffer : BufferView
}

type FocusedWindowView = {
    StatusLine : StatusLineView
    Area : PixelGrid.Block
    Buffer : BufferView
    Cursor : CursorView
}

[<RequireQualifiedAccess>]
type WindowView =
    | Unfocused of UnfocusedWindowView
    | Focused of FocusedWindowView

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
type OutputMessageView =
    | Text of string
    | Error of string

type MainViewModel = {
    Size : CellGrid.Dimensions
    TabBar : TabNameView list
    VisibleWindows : WindowView list
    CommandBar : CommandBarView
    OutputMessages : OutputMessageView list
}

module ViewModel =
    open Void.Util

    let defaultTitle = "Void - A text editor in the spirit of Vim"
    let defaultFontSize = 9uy

    let bufferFrom (windowSize : CellGrid.Dimensions) lines =
        let truncateToWindowWidth = StringUtil.noLongerThan windowSize.Columns
        {
            Contents = lines
            |> SeqUtil.notMoreThan windowSize.Rows
            |> Seq.map truncateToWindowWidth
            |> Seq.toList
        }

    let addMessage viewModel msg =
        { viewModel with OutputMessages = msg :: viewModel.OutputMessages }
