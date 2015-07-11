namespace Void.Core

type Message = interface end

[<AutoOpen>]
module ``This module is auto-opened to provide a null message`` =
    type private NullMessage =
        | NoMsg
        interface Message
    let noMessage = NoMsg :> Message

[<RequireQualifiedAccess>]
type CoreEvent =
    | BufferAdded of int * BufferType
    | CommandEntryCancelled
    | CommandMode_CharacterBackspaced
    | CommandMode_TextAppended of string
    | EditorInitialized
    | ErrorOccurred of Error
    | FileOpenedForEditing of string * string seq
    | FileSaved of string
    | LastWindowClosed // TODO this should be in the view model
    | LineCommandCompleted
    | NewFileForEditing of string
    | NotificationAdded of UserNotification
    | ModeSet of Mode
    | ModeChanged of ModeChange
    interface Message

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
    interface Message

[<AutoOpen>]
module ``This module is auto-opened to provide message aliases`` =
    let notImplemented =
        CoreEvent.ErrorOccurred Error.NotImplemented :> Message
