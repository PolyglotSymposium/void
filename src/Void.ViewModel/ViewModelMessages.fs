namespace Void.ViewModel

open Void.Core

type SegmentOfText = {
    LeftMostCell : CellGrid.Cell
    Text : string
}

[<RequireQualifiedAccess>]
type FileOrBufferId = // TODO this is very sketchy right now
    | BufferNumber of int // #1, #2 etc
    | AlternateBuffer // #
    | CurrentBuffer // %
    | Path of string

[<RequireQualifiedAccess>]
type VMEvent =
    | CommandBar_CharacterBackspacedFromLine of CellGrid.Cell
    | CommandBar_Displayed of CommandBarView
    | CommandBar_Hidden of CommandBarView
    | CommandBar_TextAppendedToLine of SegmentOfText
    | CommandBar_TextChanged of CommandBarView
    | CommandBar_TextReflowed of CommandBarView
    | ViewModelInitialized of MainViewModel // Vim rough equivalent: GUIEnter
    | ViewPortionRendered of PointGrid.Block * seq<DrawingObject>
    | BufferLoadedIntoWindow
    interface Event

[<RequireQualifiedAccess>]
type VMCommand =
    | Edit of FileOrBufferId
    | Write of FileOrBufferId
    interface Command
