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
module vmBy =
    (* Unit of measure types seem to get erased at runtime.
     * When we need a downcast to work, we need a type that doesn't get erased.
     * Thus these By* types provide a wrapper. *)
     type ScreenHeight = ScreenHeight of int<mScreenHeight>

     let screenHeight (x : int) =
        ScreenHeight (x * 1<mScreenHeight>)

[<RequireQualifiedAccess>]
type VMCommand =
    | Edit of FileOrBufferId
    | Write of FileOrBufferId
    | Scroll of Move<By.Line>
    | ScrollHalf of Move<vmBy.ScreenHeight>
    interface CommandMessage
