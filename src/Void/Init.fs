namespace Void

open Void.Core
open Void.Lang.Interpreter
open Void.Lang.Editor
open Void.ViewModel

type InputModeChanger =
    abstract member SetInputHandler : InputMode<unit> -> unit

module Init =
    let setInputMode (changer : InputModeChanger) (publish : Message -> unit) (inputMode : InputMode<Message>) =
        match inputMode with
        | InputMode.KeyPresses handler ->
            (fun keyPress -> handler keyPress |> publish)
            |> InputMode.KeyPresses
        | InputMode.TextAndHotKeys handler ->
            (fun textOrHotKey -> handler textOrHotKey |> publish)
            |> InputMode.TextAndHotKeys
        |> changer.SetInputHandler

    let initializeVoid view inputModeChanger =
        let notificationService = NotificationService()
        let editorService = EditorService()
        let viewService = ViewService view
        let commandHandlers = [
            notificationService.handleCommand
            viewService.handleCommand
            editorService.handleCommand
        ]
        let eventHandlers = [
            notificationService.handleEvent
            viewService.handleEvent
        ]
        let broker = Broker(commandHandlers, eventHandlers)
        let interpreter = Interpreter.init <| VoidScriptEditorModule(broker.publish).Commands
        let interpreterWrapper = InterpreterWrapperService interpreter
        let modeService = ModeService(NormalModeInputHandler(),
                                      CommandModeInputHandler interpreterWrapper.interpretFragment,
                                      VisualModeInputHandler(),
                                      InsertModeInputHandler(),
                                      setInputMode inputModeChanger broker.publish)
        broker.addCommandHandler modeService.handleCommand
        broker.addEventHandler modeService.handleEvent

        broker.publish Command.InitializeVoid
