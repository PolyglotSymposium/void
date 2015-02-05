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
    let originCell = { Row = 1us; Column = 1us }

    let rightOf cell count =
        { Row = cell.Row; Column = cell.Column + count }

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

type ViewSize = {
    Dimensions : CellGrid.Dimensions
    FontMetrics : PixelGrid.FontMetrics
}

module Sizing =
    open System
    open PixelGrid
    open CellGrid

    let private ceiling (x : uint16) =
        Convert.ToDouble x |> Math.Ceiling |> Convert.ToUInt16

    let defaultViewSize =
        {
            Dimensions = { Rows = 26us; Columns = 80us }
            FontMetrics = { LineHeight = 10us; CharWidth = 5us } // Arbitrary default
        }

    let viewSizeInPixels viewSize = 
        {
            Height = ceiling viewSize.FontMetrics.LineHeight * viewSize.Dimensions.Rows
            Width = ceiling viewSize.FontMetrics.CharWidth * viewSize.Dimensions.Columns
        }

[<RequireQualifiedAccess>]
type CursorView =
    | Block of CellGrid.Cell
    | IBeam of PixelGrid.Point

[<RequireQualifiedAccess>]
type StatusLineView = // TODO much yet to be done here
    | Unfocused
    | Focused

type UnfocusedWindowView = {
    StatusLine : StatusLineView
    Area : PixelGrid.Block
}

type FocusedWindowView = {
    StatusLine : StatusLineView
    Area : PixelGrid.Block
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

type TopLevelViewModel = {
    TabBar : TabNameView list
    VisibleWindows : WindowView list
    CommandBar : CommandBarView
    OutputMessages : OutputMessageView list
}

module ViewModel =
    let addMessage viewModel msg =
        { viewModel with OutputMessages = msg :: viewModel.OutputMessages }