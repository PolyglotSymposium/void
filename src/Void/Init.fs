namespace Void

open Void.Core
open Void.Lang.Interpreter
open Void.Lang.Editor
open Void.ViewModel

type MessagingSystem = {
    CoreEventChannel : Channel<CoreEvent>
    CoreCommandChannel : Channel<CoreCommand>
    CommandModeEventChannel : Channel<CommandMode.Event>
    FilesystemCommandChannel : Channel<Filesystem.Command>
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
        let commandBarHandleEvent, commandBarHandleCommandModeEvent = CommandBarService.build()
        let windowBufferMapEventHandler, windowBufferMapCommandHandler = WindowBufferMap.Service.build()
        let viewService = ViewModelService()
        let coreCommandChannel =
            Channel [
                bufferListCommandHandler
                notificationServiceCommandHandler
                viewService.handleCommand
                editorService.handleCommand
            ]
        let filesystemCommandChannel = Channel [ Filesystem.handleCommand ]
        let coreEventChannel =
            Channel [
                bufferListEventHandler
                NotifyUserOfEvent.handleEvent
                notificationServiceEventHandler
                windowBufferMapEventHandler
                commandBarHandleEvent
                viewService.handleEvent
            ]
        let commandModeEventChannel =
            Channel [
                commandBarHandleCommandModeEvent
            ]
        let vmEventChannel =
            Channel [
            ]
        let commandBarEventChannel =
            Channel [
                viewService.handleCommandBarEvent
            ]
        let vmCommandChannel =
            Channel [
                windowBufferMapCommandHandler
            ]
        let bus =
            Bus [
                coreCommandChannel.publish
                filesystemCommandChannel.publish
                coreEventChannel.publish
                commandBarEventChannel.publish
                commandModeEventChannel.publish
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
        coreCommandChannel.addHandler modeService.handleCommand
        coreEventChannel.addHandler modeService.handleEvent
        commandModeEventChannel.addHandler modeService.handleCommandModeEvent

        {
            CoreEventChannel = coreEventChannel
            CoreCommandChannel = coreCommandChannel
            CommandModeEventChannel = commandModeEventChannel
            FilesystemCommandChannel = filesystemCommandChannel
            VMEventChannel = vmEventChannel
            VMCommandChannel = vmCommandChannel
            Bus = bus
        }

    let launchVoid messagingSystem =
        messagingSystem.Bus.publish CoreCommand.InitializeVoid