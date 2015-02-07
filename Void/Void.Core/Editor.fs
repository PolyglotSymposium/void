namespace Void.Core

[<RequireQualifiedAccess>]
type Mode =
    | Insert
    | Normal
    | Command

// TODO This is naive, obviously
type FileBuffer = {
    Filepath : string option
    Contents: string list
}

[<RequireQualifiedAccess>]
type Buffer =
    | Empty
    | File of FileBuffer
    | Scratch
    | Shell
