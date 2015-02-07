namespace Void.ViewModel

open Void.Core
open System

type Artist =
    abstract member RenderText : ScreenTextObject -> unit
    abstract member RenderBlock : ScreenBlockObject -> unit

type MainView =
    abstract member Close : unit -> unit
    abstract member GetFontMetrics : unit -> PixelGrid.FontMetrics
    abstract member SetBackgroundColor : RGBColor -> unit
    abstract member SetFontBySize : byte -> unit
    abstract member SetViewSize : PixelGrid.Dimensions -> unit
    abstract member SetViewTitle : string -> unit
    abstract member SubscribeToKeyUp : Action<KeyPress> -> unit
    abstract member SubscribeToDraw : Action<Artist> -> unit
    abstract member Redraw : unit -> unit
