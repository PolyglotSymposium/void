namespace Void.Core

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

type HandleRequest<'TRequest when 'TRequest :> RequestMessage> = 
    'TRequest -> ResponseMessage<'TRequest>

type MaybeHandleRequest<'TRequest when 'TRequest :> RequestMessage> = 
    'TRequest -> ResponseMessage<'TRequest> option

type HandleResponse<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> = 
    'TResponse -> Message

type SubscribeToBus =
    abstract member subscribe<'TMsg when 'TMsg :> Message> : Handle<'TMsg> -> unit
    abstract member subscribeToRequest<'TRequest when 'TRequest :> RequestMessage> : HandleRequest<'TRequest> -> unit
    abstract member subscribeToRequest<'TRequest when 'TRequest :> RequestMessage> : MaybeHandleRequest<'TRequest> -> unit
    abstract member subscribeToResponse<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> : HandleResponse<'TRequest, 'TResponse> -> unit
