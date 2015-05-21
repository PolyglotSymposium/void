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
        let messageService = MessageService()
        let editorService = EditorService()
        let viewService = ViewService view
        let commandHandlers = [
            messageService.handleCommand
            viewService.handleCommand
            editorService.handleCommand
        ]
        let eventHandlers = [
            messageService.handleEvent
            viewService.handleEvent
        ]
        let broker = Broker(commandHandlers, eventHandlers)
        let interpreter = Interpreter.init <| VoidScriptEditorModule(broker.publishCommand).Commands
        let interpreterWrapper = InterpreterWrapperService interpreter
        let modeService = ModeService(NormalModeInputHandler(),
                                      CommandModeInputHandler interpreterWrapper.interpretFragment,
                                      VisualModeInputHandler(),
                                      InsertModeInputHandler(),
                                      setInputMode inputModeChanger broker.publishCommand)
        broker.addCommandHandler modeService.handleCommand
        broker.addEventHandler modeService.handleEvent

        broker.publishCommand Command.InitializeVoid
