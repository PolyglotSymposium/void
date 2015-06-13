namespace Void.ViewModel

open Void.Core

type ScreenLineObject = {
    StartingPoint : PointGrid.Point
    EndingPoint : PointGrid.Point
}

type ScreenTextObject = {
    Text : string
    UpperLeftCorner : PointGrid.Point
    Color : RGBColor
}

type ScreenBlockObject = {
    Area : PointGrid.Block
    Color : RGBColor
}

[<RequireQualifiedAccess>]
type DrawingObject =
    | Line of ScreenLineObject
    | Text of ScreenTextObject
    | Block of ScreenBlockObject
