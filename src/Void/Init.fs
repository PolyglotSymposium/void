namespace Void

open Void.Core
open Void.Lang.Interpreter
open Void.Lang.Editor
open Void.ViewModel

type MessagingSystem = {
    EventChannel : Channel<Event>
    CommandChannel : Channel<Command>
    VMEventChannel : Channel<VMEvent>
    Bus : Bus
}

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

    let buildVoid inputModeChanger =
        let notificationService = NotificationService()
        let editorService = EditorService()
        let viewService = ViewModelService()
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
        let vmEventChannel = Channel []
        let bus =
            Bus [
                commandChannel.publish
                eventChannel.publish
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

        {
            EventChannel = eventChannel
            CommandChannel = commandChannel
            VMEventChannel = vmEventChannel
            Bus = bus
        }

    let launchVoid messagingSystem =
        messagingSystem.Bus.publish Command.InitializeVoid