namespace Void.Core

// TODO be very careful to get the abstractions right here!
// TODO could be very easy to shoot oneself in the foot with the wrong abstraction!
[<RequireQualifiedAccess>]
type Mode =
    | Insert
    | Normal
    | Command
    | Visual
    | VisualBlock // TODO should this be subsumed under Visual?
    | OperatorPending
    // TODO there are many more modes

// TODO This is naive, obviously
type FileBuffer = {
    Filepath : string option
    Contents : string list
}

[<RequireQualifiedAccess>]
type Buffer =
    | Empty // TODO is this needed?
    | File of FileBuffer
    | Scratch
    | Shell
