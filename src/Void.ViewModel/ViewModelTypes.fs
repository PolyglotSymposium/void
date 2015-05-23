namespace Void.ViewModel

open Void.Core

[<RequireQualifiedAccess>]
type CursorView =
    | Block of CellGrid.Cell
    | IBeam of PointGrid.Point
    | Hidden

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
    Cursor : CursorView
}

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
type UserNotificationView =
    | Text of string
    | Error of string

type MainViewModel = {
    Size : CellGrid.Dimensions
    TabBar : TabNameView list
    VisibleWindows : WindowView list
    CommandBar : CommandBarView // for command mode
    Notifications : UserNotificationView list
}
