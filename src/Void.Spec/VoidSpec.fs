namespace Void.Spec

open Void
open Void.Core
open Void.ViewModel
open NUnit.Framework
open FsUnit

type MainViewStub() =
    member val Closed = false with get, set
    member val PaintedObjects = 0 with get, set
    member val Bus = Bus([]) with get, set
    interface MainView with
        member x.Close() =
            x.Closed <- true
        member x.GetFontMetrics() =
            { LineHeight = 0; CharWidth = 0 }
        member x.SetBackgroundColor color =
            ()
        member x.SetFontBySize size =
            ()
        member x.SetViewSize dimensions =
            ()
        member x.SetViewTitle title =
            ()
        member x.TriggerDraw block =
            x.Bus.publish <| VMEvent.PaintInitiated (fun _ -> x.PaintedObjects <- x.PaintedObjects + 1)

type InputModeChangerStub() =
    let mutable _inputHandler = InputMode<unit>.KeyPresses (fun _ -> ())
    member x.getInputHandler() =
        _inputHandler
    interface InputModeChanger with
        member x.SetInputHandler inputMode =
            _inputHandler <- inputMode

[<TestFixture>]
type ``Void``() = 
    [<Test>]
    member x.``When I have freshly opened Vim with one window, when I enter the quit command, then the editor exists``() =
        let mainView = MainViewStub()
        let inputModeChanger = InputModeChangerStub()
        Init.initializeVoid mainView inputModeChanger |> ignore

        match inputModeChanger.getInputHandler() with
        | InputMode.KeyPresses handler ->
            handler KeyPress.Semicolon
        | InputMode.TextAndHotKeys _ ->
            Assert.Fail "Right after startup did not have a key presses handler set"

        match inputModeChanger.getInputHandler() with
        | InputMode.KeyPresses _ ->
            Assert.Fail "After typing ; did not have text and hotkeys handler set"
        | InputMode.TextAndHotKeys handler ->
            TextOrHotKey.Text "q" |> handler
            TextOrHotKey.HotKey HotKey.Enter |> handler

        mainView.Closed |> should be True

    [<Test>]
    member x.``When I type CTRL-L in normal mode, the screen is redrawn``() =
        let mainView = MainViewStub()
        let inputModeChanger = InputModeChangerStub()
        mainView.Bus <- Init.initializeVoid mainView inputModeChanger
        mainView.PaintedObjects |> should equal 0

        match inputModeChanger.getInputHandler() with
        | InputMode.KeyPresses handler ->
            handler KeyPress.ControlL
        | InputMode.TextAndHotKeys _ ->
            Assert.Fail "Right after startup did not have a key presses handler set"

        mainView.PaintedObjects |> should be (greaterThan 20)