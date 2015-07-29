namespace Void.Core

[<RequireQualifiedAccess>]
type InputMode<'Output> =
    | KeyPresses of (KeyPress -> 'Output)
    | TextAndHotKeys of (TextOrHotKey -> 'Output)

type VisualModeInputHandler() =
    member x.handleKeyPress whatever =
        notImplemented

type InsertModeInputHandler() =
    member x.handleTextOrHotKey whatever =
        notImplemented

type ModeNotImplementedYet_FakeInputHandler() =
    member x.handleAnything whatever =
        notImplemented

type ModeService
    (
        normalModeInputHandler : NormalModeBindings.InputHandler,
        commandModeInputHandler : CommandMode.InputHandler,
        visualModeInputHandler : VisualModeInputHandler,
        insertModeInputHandler : InsertModeInputHandler,
        setInputMode : InputMode<Message> -> unit
    ) =
    let mutable _mode = Mode.Normal

    let inputHandlerFor =
        function
        | Mode.Normal ->
            InputMode.KeyPresses normalModeInputHandler.handleKeyPress
        | Mode.Command ->
            InputMode.TextAndHotKeys commandModeInputHandler.handleTextOrHotKey
        | Mode.Visual ->
            InputMode.KeyPresses visualModeInputHandler.handleKeyPress
        | Mode.Insert ->
            InputMode.TextAndHotKeys insertModeInputHandler.handleTextOrHotKey
        | _ ->
            ModeNotImplementedYet_FakeInputHandler().handleAnything
            |> InputMode.KeyPresses

    member x.handleEvent =
        function
        | CoreEvent.ErrorOccurred (Error.ScriptFragmentParseFailed _) -> 
            CoreCommand.ChangeToMode Mode.Normal :> Message // TODO or whatever mode we were in previously?
        | _ -> noMessage

    member x.handleCommandModeEvent =
        function
        | CommandMode.Event.CommandCompleted _ -> 
            if _mode = Mode.Command
            then CoreCommand.ChangeToMode Mode.Normal :> Message // TODO or whatever mode we were in previously?
            else noMessage
        | CommandMode.Event.EntryCancelled ->
            CoreCommand.ChangeToMode Mode.Normal :> Message // TODO or whatever mode we were in previously?
        | _ -> noMessage

    member x.handleCommand =
        function
        | CoreCommand.InitializeVoid ->
            setInputMode <| inputHandlerFor _mode
            CoreEvent.ModeSet _mode :> Message
        | CoreCommand.ChangeToMode mode ->
            let change = { From = _mode; To = mode }
            _mode <- change.To
            setInputMode <| inputHandlerFor change.To
            CoreEvent.ModeChanged change :> Message
        | _ -> noMessage

    member x.subscribe (subscribeHandler : SubscribeToBus) =
        subscribeHandler.subscribe x.handleCommandModeEvent
        subscribeHandler.subscribe x.handleEvent
        subscribeHandler.subscribe x.handleCommand
        commandModeInputHandler.subscribe subscribeHandler
        subscribeHandler.subscribe normalModeInputHandler.handleCommand
