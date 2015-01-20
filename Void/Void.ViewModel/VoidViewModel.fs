namespace Void.ViewModel

open System

type ViewDimensions = {
    Rows : uint16
    Columns : uint16
}

type RGBColor = {
    Red : byte
    Green : byte
    Blue : byte
}

type FontMetrics = {
    LineHeight : float
    CharWidth : float
}

type SizeInPixels = {
    Height : uint16
    Width : uint16
}

module View =
    type Point = {
        X : float
        Y : float
    }
    type Block = {
        UpperLeftCorner : Point
        Height : float
        Width : float
    }

module Colors =
    let white = { Red = 255uy; Green = 255uy; Blue = 255uy }
    let black = { Red = 0uy; Green = 0uy; Blue = 0uy }


module Sizing =
    let ceilingAsUInt16 (x : float) =
        Math.Ceiling(x) |> Convert.ToUInt16

    let viewSizeFromFontMetrics viewDimensions fontMetrics = 
        {
            Height = ceilingAsUInt16 (fontMetrics.LineHeight * (float viewDimensions.Rows))
            Width = ceilingAsUInt16 (fontMetrics.CharWidth * (float viewDimensions.Columns))
        }

module Scope = 
    let bootstrapped = true