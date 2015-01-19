namespace Void.ViewModel

open Void.Core

type MainView =
    abstract member GetFontMetrics : unit -> FontMetrics
    abstract member SetBackgroundColor : RGBColor -> unit
    abstract member SetFontBySize : byte -> unit
    abstract member SetViewSize : SizeInPixels -> unit
    abstract member SetViewTitle : string -> unit
    abstract member SubscribeToKeyUp : System.Action<KeyPress> -> unit
    abstract member Close : unit -> unit

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

type MainController
    (
        _view : MainView
    ) =
    let _dimensions = { Rows = 25us; Columns = 80us }
    let _normalCtrl = NormalModeController()

    member x.init() =
        _view.SetViewTitle "Void - A text editor in the spirit of Vim"
        _view.SetBackgroundColor Colors.black
        _view.SetFontBySize 9uy;

        let fontMetrics = _view.GetFontMetrics()
        Sizing.viewSizeFromFontMetrics _dimensions fontMetrics
        |> _view.SetViewSize 

        _view.SubscribeToKeyUp (fun keyPress ->
            _normalCtrl.handle keyPress |> x.handle
        )

    member x.handle command =
        match command with
        | Command.Noop -> ()
        | Command.Quit -> _view.Close()
        | _ -> () // TODO implement all command explicitly

    // TODO remove
    member x.foregroundColor() =
        Colors.white