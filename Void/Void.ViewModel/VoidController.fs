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
    abstract member SetViewSize : PixelGrid.IntegerDimensions -> unit
    abstract member SetViewTitle : string -> unit
    abstract member SubscribeToKeyUp : Action<KeyPress> -> unit
    abstract member SubscribeToDraw : Action<Artist> -> unit
    abstract member Redraw : unit -> unit

type NormalModeController() =
    let _bindings = NormalMode.defaultBindings
    let mutable _state = NormalMode.noKeysYet

    member x.handle keyPress =
        match NormalMode.parse _bindings keyPress _state with
        | NormalMode.ParseResult.AwaitingKeyPress prevKeys ->
            _state <- prevKeys
            Command.Noop
        | NormalMode.ParseResult.Command command ->
            command

type ViewController = TODO

open CellGrid

type MainController
    (
        _view : MainView
    ) =
    let _dimensions = { Rows = 25us; Columns = 80us }
    let _normalCtrl = NormalModeController()
    let _colorscheme = Colors.defaultColorscheme

    member x.init() =
        _view.SetViewTitle "Void - A text editor in the spirit of Vim"
        _view.SetBackgroundColor Colors.black
        _view.SetFontBySize 9uy;

        let fontMetrics = _view.GetFontMetrics()
        Sizing.viewSizeFromFontMetrics _dimensions fontMetrics
        |> _view.SetViewSize 

        // TODO refactor to architecture
        _view.SubscribeToDraw(fun artist ->
            let offset25 = fontMetrics.LineHeight * (_dimensions.Rows - 1us |> float)
            artist.RenderText "XWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWX" { X = 2.0; Y = 0.0 } _colorscheme.Foreground
            for i in [1us.._dimensions.Rows - 2us] do
                let offset = fontMetrics.LineHeight * (float i)
                artist.RenderText (sprintf "X Line #%i" i) { X = 2.0; Y = offset } _colorscheme.Foreground
            artist.RenderText "XWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWX" { X = 2.0; Y = offset25 } _colorscheme.Foreground
        )

        _view.SubscribeToKeyUp (fun keyPress ->
            _normalCtrl.handle keyPress |> x.handle
        )

    member x.handle command =
        match command with
        | Command.Noop -> ()
        //| Command.ChangeToMode mode
        | Command.Quit -> _view.Close()
        | Command.Redraw -> _view.Redraw()
        | _ -> () // TODO implement all command explicitly