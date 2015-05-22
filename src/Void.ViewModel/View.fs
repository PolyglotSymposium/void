namespace Void.ViewModel

open Void.Core.PointGrid

type MainView =
    // TODO Turn most or all of these into VMCommands?
    abstract member SetBackgroundColor : RGBColor -> unit
    abstract member SetFontBySize : byte -> unit
    abstract member SetViewSize : Dimensions -> unit
    abstract member SetViewTitle : string -> unit
    abstract member TriggerDraw : Block -> unit
