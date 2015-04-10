namespace Void

open Void.Core
open Void.Lang.Interpreter
open Void.Lang.Editor
open Void.ViewModel

type InputModeChanger =
    abstract member SetInputHandler : InputMode<unit> -> unit

module Init =
    let setInputMode (changer : InputModeChanger) (publishCommand : Command -> unit) (inputMode : InputMode<Command>) =
        match inputMode with
        | InputMode.KeyPresses handler ->
            (fun keyPress -> handler keyPress |> publishCommand)
            |> InputMode.KeyPresses
        | InputMode.TextAndHotKeys handler ->
            (fun textOrHotKey -> handler textOrHotKey |> publishCommand)
            |> InputMode.TextAndHotKeys
        |> changer.SetInputHandler

    let initializeVoid view inputModeChanger =
        let messageCtrl = MessageController()
        let editorCtrl = EditorController()
        let viewCtrl = ViewController view
        let commandHandlers = [
            messageCtrl.handleCommand
            viewCtrl.handleCommand
            editorCtrl.handleCommand
        ]
        let eventHandlers = [
            messageCtrl.handleEvent
            viewCtrl.handleEvent
        ]
        let broker = Broker(commandHandlers, eventHandlers)
        let interpreter = Interpreter.init <| VoidScriptEditorModule(broker.publishCommand).Commands
        let interpreterWrapper = InterpreterWrapperController interpreter
        let modeCtrl = ModeController(NormalModeInputHandler(),
                                      CommandModeInputHandler interpreterWrapper.interpretFragment,
                                      VisualModeInputHandler(),
                                      InsertModeInputHandler(),
                                      setInputMode inputModeChanger broker.publishCommand)
        broker.addCommandHandler modeCtrl.handleCommand
        broker.addEventHandler modeCtrl.handleEvent

        broker.publishCommand Command.InitializeVoid
        //broker.brokerCommand Command.ViewTestBuffer // TODO for testing and debugging only
