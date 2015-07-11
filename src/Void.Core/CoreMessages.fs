namespace Void.Core

[<RequireQualifiedAccess>]
type CoreEvent =
    | BufferAdded of int * BufferType
    | EditorInitialized
    | ErrorOccurred of Error
    | FileOpenedForEditing of string * string seq
    | FileSaved of string
    | LastWindowClosed // TODO this should be in the view model
    | NewFileForEditing of string
    | NotificationAdded of UserNotification
    | ModeSet of Mode
    | ModeChanged of ModeChange
    interface EventMessage

type Displayable =
    | Notifications of UserNotification list

[<RequireQualifiedAccess>]
type EditorOption =
    | ReadOnly

[<RequireQualifiedAccess>]
type CoreCommand =
    | AddNotification of UserNotification
    | ChangeToMode of Mode
    | Display of Displayable
    | Echo of string
    | FormatCurrentLine
    | Help
    | InitializeVoid
    | Put
    | Quit
    | QuitAll
    | QuitAllWithoutSaving
    | QuitWithoutSaving
    | Redraw
    | SetBufferOption of EditorOption
    | ShowNotificationHistory
    | WriteBuffer of int
    | WriteBufferToPath of int * string
    | Yank
    interface CommandMessage

[<AutoOpen>]
module ``This module is auto-opened to provide message aliases`` =
    let notImplemented =
        CoreEvent.ErrorOccurred Error.NotImplemented :> Message
