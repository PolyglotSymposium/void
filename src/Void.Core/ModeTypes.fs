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
    | OperatorPending // TODO is this a submode of command
    // TODO there are many more modes

type ModeChange = {
    From : Mode
    To : Mode
}
