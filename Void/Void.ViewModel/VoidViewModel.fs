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
        DimensionsF : Dimensions
    }

    let origin = { X = 0us; Y = 0us }

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
