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

    let buildVoid inputModeChanger =
        let editorService = EditorService()
        let viewService = ViewModelService()
        let coreCommandChannel =
            Channel [
                viewService.handleCommand
                editorService.handleCommand
            ]
        let coreEventChannel =
            Channel [
                viewService.handleEvent
            ]
        let commandModeEventChannel = Channel [] :> Channel<CommandMode.Event>
        let vmEventChannel =
            Channel [
                viewService.handleVMEvent
            ]
        let bus =
            Bus [
                coreCommandChannel
                coreEventChannel
                commandModeEventChannel
                vmEventChannel
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

        BufferList.Service.subscribe bus
        Notifications.Service.subscribe bus
        Filesystem.Service.subscribe bus
        CommandBar.Service.subscribe bus
        WindowBufferMap.Service.subscribe bus
        NotifyUserOfEvent.Service.subscribe bus
        bus

    let launchVoid (bus : Bus) =
        bus.publish CoreCommand.InitializeVoid