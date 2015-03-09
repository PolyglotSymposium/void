namespace Void.Core

[<RequireQualifiedAccess>]
type Event =
    | BufferLoadedIntoWindow of BufferType
    | EditorInitialized of EditorState
    | ErrorOccurred of Error
    | LastWindowClosed
    | MessageAdded of Message
    | ModeSet of Mode
    | ViewInitialized // Vim equivalent: GUIEnter
