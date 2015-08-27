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
    | ViewModelInitialized of MainViewModel // Vim rough equivalent: GUIEnter
    | ViewPortionRendered of PointGrid.Block * seq<DrawingObject>
    | BufferLoadedIntoWindow
    interface EventMessage

[<Measure>] type mScreenHeight

[<RequireQualifiedAccess>]
type VMCommand =
    | Edit of FileOrBufferId
    | Write of FileOrBufferId
    | Move of Motion
    | Scroll of Move<mLine>
    | ScrollHalf of Move<mScreenHeight>
    interface CommandMessage
