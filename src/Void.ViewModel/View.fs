namespace Void.ViewModel

open System
open Void.Core

type MainView =
    abstract member Close : unit -> unit
    abstract member GetFontMetrics : unit -> PixelGrid.FontMetrics
    abstract member SetBackgroundColor : RGBColor -> unit
    abstract member SetFontBySize : byte -> unit
    abstract member SetViewSize : PixelGrid.Dimensions -> unit
    abstract member SetViewTitle : string -> unit
    abstract member SubscribeToPaint : (Action<DrawingObject> -> unit) -> unit
    abstract member TriggerDraw : PixelGrid.Block -> unit
