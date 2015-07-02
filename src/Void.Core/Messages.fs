namespace Void.Core

type Message = interface end

[<AutoOpen>]
module ``This module is auto-opened to provide a null message`` =
    type private NullMessage =
        | NoMsg
        interface Message
    let noMessage = NoMsg :> Message

[<RequireQualifiedAccess>]
type Event =
    | BufferAdded of int
    | CommandEntryCancelled
    | CommandMode_CharacterBackspaced
    | CommandMode_TextAppended of string
    | EditorInitialized of BufferList // TODO this event is too big
    | ErrorOccurred of Error
    | FileOpenedForEditing of string seq
    | FileOpenedForViewing of string seq
    | FileSaved of string
    | FileSaveFailed of Error
    | LastWindowClosed // TODO this should be in the view model
    | LineCommandCompleted
    | NewFileForEditing of string
    | NewFileForViewing of string
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
type Command =
    | ChangeToMode of Mode
    | Display of Displayable
    | Echo of string
    | OpenFile of string
    | FormatCurrentLine
    | Help
    | InitializeVoid
    | Put
    | Quit
    | QuitAll
    | QuitAllWithoutSaving
    | QuitWithoutSaving
    | Redraw
    | SaveToDisk of string * string seq
    | SetBufferOption of EditorOption
    | ShowNotificationHistory
    | Write of FileIdentifier
    | Yank
    interface Message

[<AutoOpen>]
module ``This module is auto-opened to provide message aliases`` =
    let notImplemented =
        Event.ErrorOccurred Error.NotImplemented :> Message
