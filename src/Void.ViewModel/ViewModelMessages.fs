namespace Void.ViewModel

open Void.Core

type SegmentOfText = {
    LeftMostCell : CellGrid.Cell
    Text : string
}

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
    interface Message
