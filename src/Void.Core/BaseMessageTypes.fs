﻿namespace Void.Core

type Message = interface end

type CommandMessage = inherit Message
type EventMessage = inherit Message

type EnvelopeMessage<'TInnerMessage when 'TInnerMessage :> Message> = inherit Message

type RequestMessage = inherit Message
type ResponseMessage<'TRequest when 'TRequest :> RequestMessage> = inherit Message

[<AutoOpen>]
module ``This module is auto-opened to provide a null message`` =
    type NoMessage =
        | NoMessage
        interface Message
    let noMessage = NoMessage :> Message

type NoResponseToRequest<'TRequest when 'TRequest :> RequestMessage> =
    {
        Request : 'TRequest
    }
    interface Message

type Handle<'TMsg when 'TMsg :> Message> =
    'TMsg -> Message

type HandleRequest<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> = 
    'TRequest -> 'TResponse

type MaybeHandleRequest<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> = 
    'TRequest -> 'TResponse option

type HandleResponse<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> = 
    'TResponse -> Message

type SubscribeToBus =
    abstract member subscribe<'TMsg when 'TMsg :> Message> : Handle<'TMsg> -> unit
    abstract member subscribeToRequest<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> : HandleRequest<'TRequest, 'TResponse> -> unit
    abstract member subscribeToRequest<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> : MaybeHandleRequest<'TRequest, 'TResponse> -> unit
