namespace Void.ViewModel

open Void.Core

type SegmentOfText = {
    LeftMostCell : CellGrid.Cell
    Text : string
}

[<RequireQualifiedAccess>]
type VMEvent =
    | CommandBar_CharacterBackspacedFromLine of CellGrid.Cell
    | CommandBar_Displayed of CommandBarViewV2
    | CommandBar_Hidden
    | CommandBar_TextAppendedToLine of SegmentOfText
    | CommandBar_TextChanged of CommandBarViewV2
    | CommandBar_TextReflowed of CommandBarViewV2
    | ViewModelInitialized of MainViewModel // Vim rough equivalent: GUIEnter
    | ViewPortionRendered of PointGrid.Block * seq<DrawingObject>
    interface Message
