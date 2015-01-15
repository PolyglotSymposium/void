namespace Void.ViewModel

type MainView =
    abstract member GetFontMetrics : unit -> FontMetrics
    abstract member SetBackgroundColor : RGBColor -> unit
    abstract member SetFontBySize : byte -> unit
    abstract member SetViewSize : SizeInPixels -> unit
    abstract member SetViewTitle : string -> unit

type MainController
    (
        _view : MainView
    ) =
    let _dimensions = { Rows = 25us; Columns = 80us }

    member x.init() =
        _view.SetViewTitle "Void - A text editor in the spirit of Vim"
        _view.SetBackgroundColor Colors.black
        _view.SetFontBySize 9uy;
        let fontMetrics = _view.GetFontMetrics()
        Sizing.viewSizeFromFontMetrics _dimensions fontMetrics
        |> _view.SetViewSize 

    member x.foregroundColor() =
        Colors.white