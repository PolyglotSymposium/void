namespace Void

open Void.Core
open Void.ViewModel

type Channel =
    abstract member publish : Message -> Message seq
    (* F# Why you no have type classes like Haskell!?!?!
     * Now I will do ugly things, with long names! *)
    abstract member getBoxedSubscribeActionIfTypeIs<'TMsg> : unit -> obj option
    
type Channel<'TIn when 'TIn :> Message>
    (
        handlers : Handle<'TIn> list
    ) =
    let mutable _handlers = handlers

    member private x.safetyWrap handle message =
        try
            handle message
        with ex ->
            printf "Error while handling %A: %A" message ex
            noMessage

    member x.addHandler handler =
        _handlers <- x.safetyWrap handler :: _handlers

    interface Channel with
        member x.publish (message : Message) =
            match message with
                | :? 'TIn as msg ->
                    Seq.map (fun handle -> handle msg) _handlers
                    |> Seq.filter (fun msg -> msg <> noMessage)
                | _ -> Seq.empty

        member x.getBoxedSubscribeActionIfTypeIs<'TMsg>() =
            if typeof<'TIn> = typeof<'TMsg>
            then Some <| box x.addHandler
            else None

type RequestChannel =
    (* F# Why you no have type classes like Haskell!?!?!
     * Now I will do ugly things, with long names! *)
    abstract member getBoxedRequestFunctionIfResponseTypeIs<'TMsg> : unit -> obj option
    abstract member getBoxedSubscribeActionIfResponseTypeIs<'TMsg> : unit -> obj option

type RequestChannel<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>>
    (
        handlers : MaybeHandleRequest<'TRequest, 'TResponse> list
    ) =
    let mutable _handlers = handlers

    member private x.safetyWrap handle message =
        try
            handle message
        with ex ->
            printf "Error while handling %A: %A" message ex
            None

    member x.addHandler handler =
        _handlers <- x.safetyWrap handler :: _handlers

    member x.makeRequest requestMsg =
        Seq.tryPick (fun handle -> handle requestMsg) handlers

    interface RequestChannel with
        member x.getBoxedRequestFunctionIfResponseTypeIs<'TMsg>() =
            if typeof<'TResponse> = typeof<'TMsg>
            then Some <| box x.makeRequest
            else None

        member x.getBoxedSubscribeActionIfResponseTypeIs<'TMsg>() =
            if typeof<'TResponse> = typeof<'TMsg>
            then Some <| box x.addHandler
            else None

type BusImpl
    (
        channels : Channel list
    ) =
    let mutable _requestChannels = []
    let mutable _channels = channels

    member private x.addChannel channel =
        _channels <- channel :: _channels

    member private x.addRequestChannel (requestChannel : RequestChannel) =
        _requestChannels <- requestChannel :: _requestChannels

    member x.publishAll messages =
        for message in messages do
            x.publish message

    member x.publish (message : Message) =
        if message <> noMessage
        then
            for channel in _channels do
                channel.publish message |> x.publishAll

    member x.makeRequest<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> request =
        let tryGetRequestFunction (channel : RequestChannel) =
            channel.getBoxedRequestFunctionIfResponseTypeIs<'TResponse>()
        match List.tryPick tryGetRequestFunction _requestChannels with
        | Some makeRequest ->
            request
            |> unbox<MaybeHandleRequest<'TRequest, 'TResponse>> makeRequest
        | None ->
            None

    member x.subscribe<'TMsg when 'TMsg :> Message> (handle : Handle<'TMsg>) =
        let tryGetSubscribeAction (channel : Channel) =
            channel.getBoxedSubscribeActionIfTypeIs<'TMsg>()
        match List.tryPick tryGetSubscribeAction _channels with
        | Some subscribe ->
            handle
            |> unbox<Handle<'TMsg> -> unit> subscribe
        | None ->
            x.addChannel <| Channel [ handle ]

    member x.subscribeToRequest<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> (handleRequest : MaybeHandleRequest<'TRequest, 'TResponse>) =
        let tryGetSubscribeAction (channel : RequestChannel) =
            channel.getBoxedSubscribeActionIfResponseTypeIs<'TResponse>()
        match List.tryPick tryGetSubscribeAction _requestChannels with
        | Some subscribe ->
            handleRequest
            |> unbox<MaybeHandleRequest<'TRequest, 'TResponse> -> unit> subscribe
        | None ->
            x.addRequestChannel <| RequestChannel<'TRequest, 'TResponse> [ handleRequest ]

    member x.subscribeToRequest<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> (handleRequest : HandleRequest<'TRequest, 'TResponse>) =
        x.subscribeToRequest (handleRequest >> Some)

    interface Bus with
        member x.makeRequest request = x.makeRequest request
        member x.subscribe handle = x.subscribe handle
        member x.subscribeToRequest<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> (maybeHandleRequest : MaybeHandleRequest<'TRequest, 'TResponse>) = x.subscribeToRequest maybeHandleRequest
        member x.subscribeToRequest<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> (handleRequest : HandleRequest<'TRequest, 'TResponse>) = x.subscribeToRequest handleRequest
