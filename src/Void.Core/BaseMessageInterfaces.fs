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

type Handle<'TMsg when 'TMsg :> Message> =
    'TMsg -> Message

type SubscribeToBus =
    abstract member subscribe<'TMsg when 'TMsg :> Message> : Handle<'TMsg> -> unit
