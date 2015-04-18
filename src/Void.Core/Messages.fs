namespace Void.Core

[<RequireQualifiedAccess>]
type Event =
    | BufferLoadedIntoWindow of BufferType
    | CommandEntryCancelled
    | EditorInitialized of EditorState
    | ErrorOccurred of Error
    | LastWindowClosed
    | LineCommandCompleted
    | NotificationAdded of UserNotification
    | ModeSet of Mode
    | ModeChanged of ModeChange
    | ViewInitialized // Vim equivalent: GUIEnter

type Displayable =
    | Notifications of UserNotification list

[<RequireQualifiedAccess>]
type Command =
    | Noop
    | PublishEvent of Event
    //
    | ChangeToMode of Mode
    | Display of Displayable
    | Edit of FileIdentifier
    | FormatCurrentLine
    | InitializeVoid
    | Put
    | Quit
    | QuitAll
    | QuitAllWithoutSaving
    | QuitWithoutSaving
    | Redraw
    | ShowNotificationHistory
    | ViewTestBuffer // TODO for Debug/Test only
    | Yank

[<AutoOpen>]
module Util =
    let notImplemented =
        Event.ErrorOccurred Error.NotImplemented
        |> Command.PublishEvent
