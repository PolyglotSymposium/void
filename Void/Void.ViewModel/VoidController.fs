namespace Void.ViewModel

open System
open Void.Core

type Artist =
    abstract member RenderText : string -> PixelGrid.Point -> RGBColor -> unit
    abstract member RenderBlock : PixelGrid.Block -> RGBColor -> unit

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

open CellGrid

type ViewController
    (
        _view : MainView
    ) =
    let mutable _viewSize = Sizing.defaultViewSize
    let _colorscheme = Colors.defaultColorscheme

    member x.initializeView() =
        _view.SetViewTitle "Void - A text editor in the spirit of Vim"
        _view.SetBackgroundColor Colors.black
        _view.SetFontBySize 9uy;

        _viewSize <- { _viewSize with FontMetrics = _view.GetFontMetrics() }
        Sizing.viewSizeInPixels _viewSize |> _view.SetViewSize 

        // TODO refactor to architecture
        _view.SubscribeToDraw(fun artist ->
            let mutable i = 0us
            for line in Editor.testFileBuffer().Contents.Split [|'\n'|] do
                let offset = _viewSize.FontMetrics.LineHeight * i
                i <- i + 1us
                x.textOnRow artist line offset
        )

    member x.closeView() = _view.Close()

    member x.redrawEntireView() = _view.Redraw()

    member x.textOnRow artist text row =
        artist.RenderText text { X = 0us; Y = row } _colorscheme.Foreground

type MainController
    (
        _view : MainView
    ) =
    let _normalCtrl = NormalModeController()
    let _viewCtrl = ViewController(_view)

    member x.initializeVoid() =
        _viewCtrl.initializeView()
        // In general, ViewController should operate on the view, not MainController.
        // However, the lines below are input, not painting/drawing/displaying...
        _view.SubscribeToKeyUp (fun keyPress ->
            _normalCtrl.handle keyPress |> x.handle
        )

    member x.handle command =
        match command with
        | Command.Noop -> ()
        //| Command.ChangeToMode mode // TODO
        | Command.Quit -> _viewCtrl.closeView()
        | Command.Redraw -> _viewCtrl.redrawEntireView()
        | _ -> () // TODO implement all command explicitly
