namespace Void.ViewModel

open Void.Core

type RerenderWholeView = unit -> DrawingObject seq
type Draw = DrawingObject -> unit

[<RequireQualifiedAccess>]
type VMEvent =
    | PaintInitiated of Draw
    | ViewModelInitialized // Vim rough equivalent: GUIEnter
    | ViewPortionRendered of PixelGrid.Block * seq<DrawingObject>
    interface Message

[<RequireQualifiedAccess>]
type VMCommand =
    | Redraw of PixelGrid.Block
    interface Message
