namespace Void.ViewModel

open Void.Core
open System

type MainView =
    abstract member Close : unit -> unit
    abstract member GetFontMetrics : unit -> PixelGrid.FontMetrics
    abstract member SetBackgroundColor : RGBColor -> unit
    abstract member SetFontBySize : byte -> unit
    abstract member SetViewSize : PixelGrid.Dimensions -> unit
    abstract member SetViewTitle : string -> unit
    abstract member TriggerDraw : PixelGrid.Block -> unit

[<RequireQualifiedAccess>]
type ViewEvent =
    | KeyPressed of KeyPress
    | TextEntered of string
    | PaintInitiated of Action<DrawingObject>
