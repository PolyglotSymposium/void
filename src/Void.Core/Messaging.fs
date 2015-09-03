namespace Void.Core

module Messaging = 
    type private Channel =
        abstract member publish : Message -> Message seq
        (* F# Why you no have type classes like Haskell!?!?!
         * Now I will do ugly things, with long names! *)
        abstract member getBoxedSubscribeActionIfTypeIs<'TMsg> : unit -> obj option
        
    type private Channel<'TIn when 'TIn :> Message>
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

    type private RequestChannel =
        (* F# Why you no have type classes like Haskell!?!?!
         * Now I will do ugly things, with long names! *)
        abstract member getBoxedRequestFunctionIfResponseTypeIs<'TMsg> : unit -> obj option
        abstract member getBoxedSubscribeActionIfResponseTypeIs<'TMsg> : unit -> obj option

    type private RequestChannel<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>>
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

    type private PackagedRequestChannel =
        inherit RequestChannel

    type private PackagedRequestChannel<'TRequest, 'TResponse, 'TPackagedRequest, 'TPackagedResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest> and 'TPackagedRequest :> EnvelopeMessage<'TRequest> and 'TPackagedResponse :> EnvelopeMessage<'TResponse>>
        (
            handlers : MaybeHandlePackagedRequest<'TRequest, 'TResponse, 'TPackagedRequest, 'TPackagedResponse> list
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

        member x.makeRequest packagedRequestMsg =
            Seq.tryPick (fun handle -> handle packagedRequestMsg) handlers

        interface PackagedRequestChannel with
            member x.getBoxedRequestFunctionIfResponseTypeIs<'TMsg>() =
                if typeof<'TPackagedResponse> = typeof<'TMsg>
                then Some <| box x.makeRequest
                else None

            member x.getBoxedSubscribeActionIfResponseTypeIs<'TMsg>() =
                if typeof<'TPackagedResponse> = typeof<'TMsg>
                then Some <| box x.addHandler
                else None

    type private MessageRouter() =
        let mutable _channels : Channel list = []

        member x.addChannel channel =
            _channels <- channel :: _channels

        member private x.publishAll messages =
            for message in messages do
                x.publish message

        member x.publish (message : Message) =
            if message <> noMessage
            then
                for channel in _channels do
                    channel.publish message |> x.publishAll

        member x.subscribe<'TMsg when 'TMsg :> Message> (handle : Handle<'TMsg>) =
            let tryGetSubscribeAction (channel : Channel) =
                channel.getBoxedSubscribeActionIfTypeIs<'TMsg>()
            match List.tryPick tryGetSubscribeAction _channels with
            | Some subscribe ->
                handle
                |> unbox<Handle<'TMsg> -> unit> subscribe
            | None ->
                x.addChannel <| Channel [ handle ]

    type private RequestRouter() =
        let mutable _requestChannels : RequestChannel list = []

        member x.addRequestChannel requestChannel =
            _requestChannels <- requestChannel :: _requestChannels

        member x.makeRequest<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> request =
            let tryGetRequestFunction (channel : RequestChannel) =
                channel.getBoxedRequestFunctionIfResponseTypeIs<'TResponse>()
            match List.tryPick tryGetRequestFunction _requestChannels with
            | Some makeRequest ->
                request
                |> unbox<MaybeHandleRequest<'TRequest, 'TResponse>> makeRequest
            | None ->
                None

        member x.subscribeToRequest<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> (maybeHandleRequest : MaybeHandleRequest<'TRequest, 'TResponse>) =
            let tryGetSubscribeAction (channel : RequestChannel) =
                channel.getBoxedSubscribeActionIfResponseTypeIs<'TResponse>()
            match List.tryPick tryGetSubscribeAction _requestChannels with
            | Some subscribe ->
                maybeHandleRequest
                |> unbox<MaybeHandleRequest<'TRequest, 'TResponse> -> unit> subscribe
            | None ->
                x.addRequestChannel <| RequestChannel<'TRequest, 'TResponse> [ maybeHandleRequest ]

        member x.subscribeToRequest<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> (handleRequest : HandleRequest<'TRequest, 'TResponse>) =
            x.subscribeToRequest (handleRequest >> Some)

    type private PackagedRequestRouter() =
        let mutable _packagedRequestChannels : PackagedRequestChannel list = []

        member x.addPackagedRequestChannel packagedRequestChannel =
            _packagedRequestChannels <- packagedRequestChannel :: _packagedRequestChannels

        member x.makePackagedRequest<'TRequest, 'TResponse, 'TPackagedRequest, 'TPackagedResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest> and 'TPackagedRequest :> EnvelopeMessage<'TRequest> and 'TPackagedResponse :> EnvelopeMessage<'TResponse>> request =
            let tryGetRequestFunction (channel : PackagedRequestChannel) =
                channel.getBoxedRequestFunctionIfResponseTypeIs<'TPackagedResponse>()
            match List.tryPick tryGetRequestFunction _packagedRequestChannels with
            | Some makeRequest ->
                request
                |> unbox<MaybeHandlePackagedRequest<'TRequest, 'TResponse, 'TPackagedRequest, 'TPackagedResponse>> makeRequest
            | None ->
                None

        member x.subscribeToPackagedRequest<'TRequest, 'TResponse, 'TPackagedRequest, 'TPackagedResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest> and 'TPackagedRequest :> EnvelopeMessage<'TRequest> and 'TPackagedResponse :> EnvelopeMessage<'TResponse>> (maybeHandlePackagedRequest : MaybeHandlePackagedRequest<'TRequest, 'TResponse, 'TPackagedRequest, 'TPackagedResponse>) =
            let tryGetSubscribeAction (channel : RequestChannel) =
                channel.getBoxedSubscribeActionIfResponseTypeIs<'TPackagedResponse>()
            match List.tryPick tryGetSubscribeAction _packagedRequestChannels with
            | Some subscribe ->
                maybeHandlePackagedRequest
                |> unbox<MaybeHandlePackagedRequest<'TRequest, 'TResponse, 'TPackagedRequest, 'TPackagedResponse> -> unit> subscribe
            | None ->
                x.addPackagedRequestChannel <| PackagedRequestChannel<'TRequest, 'TResponse, 'TPackagedRequest, 'TPackagedResponse> [ maybeHandlePackagedRequest ]

        member x.subscribeToPackagedRequest<'TRequest, 'TResponse, 'TPackagedRequest, 'TPackagedResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest> and 'TPackagedRequest :> EnvelopeMessage<'TRequest> and 'TPackagedResponse :> EnvelopeMessage<'TResponse>> (handlePackagedRequest : HandlePackagedRequest<'TRequest, 'TResponse, 'TPackagedRequest, 'TPackagedResponse>) =
            x.subscribeToPackagedRequest (handlePackagedRequest >> Some)

    type private MessagingSystemFacade() =
        let messageRouter = MessageRouter()
        let requestRouter = RequestRouter()
        let packagedRequestRouter = PackagedRequestRouter()

        interface Bus with
            member x.publish message =
                messageRouter.publish message
            member x.makeRequest request =
                requestRouter.makeRequest request
            member x.makePackagedRequest request =
                packagedRequestRouter.makePackagedRequest request
            member x.subscribe handle =
                messageRouter.subscribe handle
            member x.subscribeToRequest<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> (maybeHandleRequest : MaybeHandleRequest<'TRequest, 'TResponse>) =
                requestRouter.subscribeToRequest maybeHandleRequest
            member x.subscribeToRequest<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> (handleRequest : HandleRequest<'TRequest, 'TResponse>) =
                requestRouter.subscribeToRequest handleRequest
            member x.subscribeToPackagedRequest<'TRequest, 'TResponse, 'TPackagedRequest, 'TPackagedResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest> and 'TPackagedRequest :> EnvelopeMessage<'TRequest> and 'TPackagedResponse :> EnvelopeMessage<'TResponse>> (handlePackagedRequest : HandlePackagedRequest<'TRequest, 'TResponse, 'TPackagedRequest, 'TPackagedResponse>) =
                packagedRequestRouter.subscribeToPackagedRequest handlePackagedRequest
            member x.subscribeToPackagedRequest<'TRequest, 'TResponse, 'TPackagedRequest, 'TPackagedResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest> and 'TPackagedRequest :> EnvelopeMessage<'TRequest> and 'TPackagedResponse :> EnvelopeMessage<'TResponse>> (maybeHandlePackagedRequest : MaybeHandlePackagedRequest<'TRequest, 'TResponse, 'TPackagedRequest, 'TPackagedResponse>) =
                packagedRequestRouter.subscribeToPackagedRequest maybeHandlePackagedRequest

    let newBus() =
        MessagingSystemFacade() :> Bus
