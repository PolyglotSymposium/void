namespace Void.Core

type Message = interface end
type Command = inherit Message
type Event = inherit Message

[<AutoOpen>]
module ``This module is auto-opened to provide a null message`` =
    type NoMessage =
        | NoMessage
        interface Message
    let noMessage = NoMessage :> Message

