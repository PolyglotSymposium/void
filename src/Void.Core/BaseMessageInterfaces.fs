namespace Void.Core

type Message = interface end
type CommandMessage = inherit Message
type EventMessage = inherit Message

[<AutoOpen>]
module ``This module is auto-opened to provide a null message`` =
    type NoMessage =
        | NoMessage
        interface Message
    let noMessage = NoMessage :> Message

