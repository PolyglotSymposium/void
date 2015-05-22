namespace Void.ViewModel

open Void.Core

type RerenderWholeView = unit -> DrawingObject seq
type Draw = DrawingObject -> unit

[<RequireQualifiedAccess>]
type VMEvent =
    | PaintInitiated of Draw
    | ViewModelInitialized // Vim rough equivalent: GUIEnter
    | ViewPortionRendered of PointGrid.Block * seq<DrawingObject>
    interface Message

[<RequireQualifiedAccess>]
type VMCommand =
    | Redraw of PointGrid.Block
    interface Message
