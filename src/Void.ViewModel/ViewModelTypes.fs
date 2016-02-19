namespace Void.ViewModel

open Void.Core

type Visibility<'a> =
    | Hidden
    | Visible of 'a

[<RequireQualifiedAccess>]
type CursorStyle =
    | Block
    | IBeam

type CursorView = {
    Position : CellGrid.Cell
    Appearance : CursorStyle Visibility
}

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
