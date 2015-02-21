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
    abstract member SubscribeToKeyUp : Action<KeyPress> -> unit
    abstract member SubscribeToDraw : Action<Action<DrawingObject>> -> unit
    abstract member TriggerDraw : unit -> unit
