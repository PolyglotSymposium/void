namespace Void.ViewModel

open Void.Core

type Visibility<'a> =
    | Hidden
    | Visible of 'a

[<RequireQualifiedAccess>]
type CursorView =
    | Block of CellGrid.Cell
    | IBeam of PointGrid.Point

[<RequireQualifiedAccess>]
type TabNameView = // Speculative... :(
    | Unfocused of string
    | Focused of string

type MainViewModel = {
    Size : CellGrid.Dimensions
    Title : string
    BackgroundColor : RGBColor
    FontSize : int
}
