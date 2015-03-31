namespace Void

open Void.Core
open Void.Lang.Interpreter
open Void.ViewModel

module Init =
    let initializeVoid view =
        let modeCtrl = ModeController()
        let messageCtrl = MessageController()
        let normalCtrl = NormalModeController()
        let editorCtrl = EditorController()
        let viewCtrl = ViewController view
        let interpreter = Interpreter.empty
        let voidScriptCtrl = VoidScriptController interpreter
        let commandHandlers = [
            messageCtrl.handleCommand
            modeCtrl.handleCommand
            viewCtrl.handleCommand
            editorCtrl.handleCommand
            voidScriptCtrl.handleCommand
        ]
        let eventHandlers = [
            messageCtrl.handleEvent
            viewCtrl.handleEvent
        ]
        let broker = Broker(commandHandlers, eventHandlers, viewCtrl, normalCtrl)
        broker.brokerCommand Command.InitializeVoid
        //broker.brokerCommand Command.ViewTestBuffer // TODO for testing and debugging only
        broker.brokerViewEvent
