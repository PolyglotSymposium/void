namespace Void.Core

[<RequireQualifiedAccess>]
type Command =
    | Noop
    | Edit
    | FormatCurrentLine
    | ChangeToMode of Mode
    | PublishEvent of Event
    | Quit
    | Redraw
    | ViewTestBuffer