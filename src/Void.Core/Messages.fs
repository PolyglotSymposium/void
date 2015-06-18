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
    | BufferLoadedIntoWindow of BufferType
    | CommandEntryCancelled
    | CommandMode_CharacterBackspaced
    | CommandMode_TextAppended of string
    | EditorInitialized of EditorState
    | ErrorOccurred of Error
    | FileOpenedForEditing of string seq
    | FileOpenedForViewing of string seq
    | FileSaved of string
    | FileSaveFailed of Error
    | LastWindowClosed
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
    | Edit of FileIdentifier
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
