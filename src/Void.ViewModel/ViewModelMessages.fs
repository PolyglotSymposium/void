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

[<RequireQualifiedAccess>]
type VMCommand =
    | Edit of FileOrBufferId
    | Write of FileOrBufferId
    interface CommandMessage
