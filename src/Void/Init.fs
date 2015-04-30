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
        let viewService = ViewModelService view
        let renderingService = RenderingService(view, viewService.rerenderWholeView)
        let commandChannel =
            Channel [
                notificationService.handleCommand
                viewService.handleCommand
                editorService.handleCommand
            ]
        let eventChannel =
            Channel [
                notificationService.handleEvent
                viewService.handleEvent
            ]
        let vmCommandChannel =
            Channel [
                renderingService.handleVMCommand
            ]
        let vmEventChannel =
            Channel [
                renderingService.handleVMEvent
            ]
        let bus =
            Bus [
                commandChannel.publish
                eventChannel.publish
                vmCommandChannel.publish
                vmEventChannel.publish
            ]
        let interpreter = Interpreter.init <| VoidScriptEditorModule(bus.publish).Commands
        let interpreterWrapper = InterpreterWrapperService interpreter
        let modeService = ModeService(NormalModeInputHandler(),
                                      CommandModeInputHandler interpreterWrapper.interpretFragment,
                                      VisualModeInputHandler(),
                                      InsertModeInputHandler(),
                                      setInputMode inputModeChanger bus.publish)
        commandChannel.addHandler modeService.handleCommand
        eventChannel.addHandler modeService.handleEvent

        bus.publish Command.InitializeVoid
        bus
