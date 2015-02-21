namespace Void.Core

[<RequireQualifiedAccess>]
type Command =
    | Noop
    | PublishEvent of Event
    //
    | ChangeToMode of Mode
    | Edit
    | FormatCurrentLine
    | Put
    | Quit
    | Redraw
    | ViewTestBuffer // TODO for Debug/Test only
    | Yank
