namespace Void.Core

[<RequireQualifiedAccess>]
type Mode =
    | Insert
    | Normal
    | Command

[<RequireQualifiedAccess>]
type Command =
    | FormatCurrentLine
    | ChangeToMode of Mode
    | Quit