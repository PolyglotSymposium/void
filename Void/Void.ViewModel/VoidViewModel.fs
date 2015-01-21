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
    type Dimensions = {
        HeightF : float
        WidthF : float
    }
    type Block = {
        UpperLeftCorner : Point
        Dimensions : Dimensions
    }

type RGBColor = {
    Red : byte
    Green : byte
    Blue : byte
}

type Colorscheme = {
    Background : RGBColor
    Foreground : RGBColor
    StatusLineEtc : RGBColor // TODO rename
    Error : RGBColor
}

module Colors =
    let white = { Red = 255uy; Green = 255uy; Blue = 255uy }
    let black = { Red = 0uy; Green = 0uy; Blue = 0uy }
    let red = { Red = 255uy; Green = 0uy; Blue = 0uy }
    let green = { Red = 0uy; Green = 255uy; Blue = 0uy }
    let blue = { Red = 0uy; Green = 0uy; Blue = 255uy }

    let defaultColorscheme = {
        Background = black
        Foreground = white
        StatusLineEtc = blue
        Error = red
    }


module Sizing =
    open System
    open PixelGrid
    open CellGrid

    let ceilingAsUInt16 (x : float) =
        Math.Ceiling(x) |> Convert.ToUInt16

    let viewSizeFromFontMetrics viewDimensions fontMetrics = 
        {
            Height = ceilingAsUInt16 (fontMetrics.LineHeight * (float viewDimensions.Rows))
            Width = ceilingAsUInt16 (fontMetrics.CharWidth * (float viewDimensions.Columns))
        }

module Scope = 
    let bootstrapped = true