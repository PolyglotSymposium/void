namespace Void

open Void.Core
open Void.Lang.Interpreter
open Void.Lang.Editor
open Void.ViewModel

type MessagingSystem = {
    EventChannel : Channel<Event>
    CommandChannel : Channel<Command>
    VMEventChannel : Channel<VMEvent>
    VMCommandChannel : Channel<VMCommand>
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
        let notificationServiceEventHandler, notificationServiceCommandHandler = Notifications.Service.build()
        let bufferListEventHandler, bufferListCommandHandler = BufferList.Service.build()
        let editorService = EditorService()
        let commandBarService = CommandBarService.build()
        let windowBufferVMCommandHandler = WindowBufferMap.Service.build()
        let viewService = ViewModelService()
        let commandChannel =
            Channel [
                bufferListCommandHandler
                notificationServiceCommandHandler
                viewService.handleCommand
                Filesystem.handleCommand
                editorService.handleCommand
            ]
        let eventChannel =
            Channel [
                bufferListEventHandler
                NotifyUserOfEvent.handleEvent
                notificationServiceEventHandler
                commandBarService
                viewService.handleEvent
            ]
        let vmEventChannel =
            Channel [
                viewService.handleVMEvent
            ]
        let vmCommandChannel =
            Channel [
                windowBufferVMCommandHandler
            ]
        let bus =
            Bus [
                commandChannel.publish
                eventChannel.publish
                vmEventChannel.publish
                vmCommandChannel.publish
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
            VMCommandChannel = vmCommandChannel
            Bus = bus
        }

    let launchVoid messagingSystem =
        messagingSystem.Bus.publish Command.InitializeVoid