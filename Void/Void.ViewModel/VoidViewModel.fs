namespace Void.ViewModel

module CellGrid =
    type Cell = {
        Row : uint16
        Column : uint16
    }
    type Dimensions = {
        Rows : uint16
        Columns : uint16
    }
    type Block = {
        UpperLeftCorner : Cell
        Dimensions : Dimensions
    }
    let originCell = { Row = 0us; Column = 0us }

    let rightOf cell count =
        { Row = cell.Row; Column = cell.Column + count }

    let below cell count =
        { Row = cell.Row + count; Column = cell.Column }

module PixelGrid =
    type FontMetrics = {
        LineHeight : uint16
        CharWidth : uint16
    }
    type Point = {
        X : uint16
        Y : uint16
    }
    type Dimensions = {
        Height : uint16
        Width : uint16
    }
    type Block = {
        UpperLeftCorner : Point
        Dimensions : Dimensions
    }

    let originPoint = { X = 0us; Y = 0us }

module Sizing =
    open System
    open PixelGrid
    open CellGrid

    let defaultViewSize = { Rows = 26us; Columns = 80us }
    let defaultFontMetrics = { LineHeight = 10us; CharWidth = 5us } // Arbitrary default

    let pointAtUpperLeftOfCell fontMetrics cell =
        {
            X = cell.Row * fontMetrics.CharWidth
            Y = cell.Column * fontMetrics.LineHeight
        }

    let cellDimensionsToPixels fontMetrics dimensions =
        {
            Height = fontMetrics.LineHeight * dimensions.Rows
            Width = fontMetrics.CharWidth * dimensions.Columns
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
    let defaultTitle = "Void - A text editor in the spirit of Vim"
    let defaultFontSize = 9uy

    let addMessage viewModel msg =
        { viewModel with OutputMessages = msg :: viewModel.OutputMessages }