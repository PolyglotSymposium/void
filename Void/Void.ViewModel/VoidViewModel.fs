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

module PixelGrid =
    type FontMetrics = {
        LineHeight : float
        CharWidth : float
    }
    type Point = {
        X : float
        Y : float
    }
    type IntegerDimensions = {
        Height : uint16
        Width : uint16
    }
    type DimensionsF = {
        HeightF : float
        WidthF : float
    }
    type Block = {
        UpperLeftCorner : Point
        DimensionsF : DimensionsF
    }

    let origin = { X = 0.0; Y = 0.0 }

type ViewSize = {
    Dimensions : CellGrid.Dimensions
    FontMetrics : PixelGrid.FontMetrics
}

module Sizing =
    open System
    open PixelGrid
    open CellGrid

    let private ceilingAsUInt16 (x : float) =
        Math.Ceiling(x) |> Convert.ToUInt16

    let defaultViewSize =
        {
            Dimensions = { Rows = 26us; Columns = 80us }
            FontMetrics = { LineHeight = 10.0; CharWidth = 5.0 } // Arbitrary default
        }

    let viewSizeInPixels viewSize = 
        {
            Height = ceilingAsUInt16 (viewSize.FontMetrics.LineHeight * (float viewSize.Dimensions.Rows))
            Width = ceilingAsUInt16 (viewSize.FontMetrics.CharWidth * (float viewSize.Dimensions.Columns))
        }
