namespace Void.Core

[<RequireQualifiedAccess>]
type Event =
    | BufferLoadedIntoWindow of Buffer
    | CoreInitialized // Vim equivalent: VimEnter (?)
    | ErrorOccurred of Error
    | MessageAdded of Message
    | ViewInitialized // Vim equivalent: GUIEnter
