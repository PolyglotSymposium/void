namespace Void.Core

[<RequireQualifiedAccess>]
type Event =
    | BufferLoadedIntoWindow of BufferType
    | EditorInitialized of EditorState
    | ErrorOccurred of Error
    | MessageAdded of Message
    | ModeSet of Mode
    | ViewInitialized // Vim equivalent: GUIEnter
