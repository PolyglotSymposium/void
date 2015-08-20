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
type StatusLineView = // TODO much yet to be done here
    | Unfocused
    | Focused

type BufferView = {
    LinesOfText: string list // TODO this is naive obviously
}

type WindowView = {
    StatusLine : StatusLineView
    Area : CellGrid.Block
    Buffer : BufferView
    Cursor : CursorView Visibility
    TopLineNumber : int<mLine>
}

(* "Command line" is too equivocal. I mean the ; (or : in Vim) bar at the
 * bottom of the screen *)
[<RequireQualifiedAccess>]
type CommandBarPrompt =
    | VoidDefault
    | ClassicVim

type CommandBarView = {
    Width : int
    Prompt : CommandBarPrompt Visibility
    WrappedLines : string list
}

[<RequireQualifiedAccess>]
type TabNameView =
    | Unfocused of string
    | Focused of string

type MainViewModel = {
    Size : CellGrid.Dimensions
    Title : string
    BackgroundColor : RGBColor
    FontSize : int
    TabBar : TabNameView list
    VisibleWindows : WindowView list
}
