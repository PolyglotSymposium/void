namespace Void.Core

[<RequireQualifiedAccess>]
type Event =
    | BufferLoadedIntoWindow of Buffer
    | ErrorOccurred of Error
    | MessageAdded of Message
