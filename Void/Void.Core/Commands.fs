namespace Void.Core

[<RequireQualifiedAccess>]
type Mode =
    | Insert
    | Normal
    | Command

[<RequireQualifiedAccess>]
type Command =
    | Noop
    | FormatCurrentLine
    | ChangeToMode of Mode
    | Quit
    | Redraw