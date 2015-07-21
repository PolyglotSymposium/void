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

    member x.addHandler handler =
        _handlers <- handler :: _handlers

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

type RequestChannel<'TRequest when 'TRequest :> RequestMessage>
    (
        handlers : HandleRequest<'TRequest> list
    ) =
    let mutable _handlers = handlers

    member x.addHandler handler =
        _handlers <- handler :: _handlers

    interface Channel with
        member x.publish (message : Message) =
            match message with
            | :? 'TRequest as msg ->
                let responses =
                    _handlers
                    |> Seq.choose (fun handle -> handle msg)
                    |> Seq.map (fun response -> response :> Message)
                if responses = Seq.empty
                then Seq.singleton<Message> { Request = msg }
                else responses
            | _ -> Seq.empty

        member x.getBoxedSubscribeActionIfTypeIs<'TMsg>() =
            if typeof<'TRequest> = typeof<'TMsg>
            then Some <| box x.addHandler
            else None

type Bus
    (
        channels : Channel list
    ) =
    let mutable _channels = channels

    member private x.addChannel channel =
        _channels <- channel :: _channels

    member private x.publishAll messages =
        for message in messages do
            x.publish message

    member x.publish (message : Message) =
        for channel in _channels do
            channel.publish message |> x.publishAll

    member x.subscribe<'TMsg when 'TMsg :> Message> (handle : Handle<'TMsg>) =
        let tryGetSubscribeAction (channel : Channel) =
            channel.getBoxedSubscribeActionIfTypeIs<'TMsg>()
        match List.choose tryGetSubscribeAction _channels with
        | [subscribe] ->
            handle
            |> unbox<Handle<'TMsg> -> unit> subscribe
        | _ ->
            x.addChannel <| Channel [ handle ]

    member x.subscribeToRequest<'TRequest when 'TRequest :> RequestMessage> (handleRequest : HandleRequest<'TRequest>) =
        let tryGetSubscribeAction (channel : Channel) =
            channel.getBoxedSubscribeActionIfTypeIs<'TRequest>()
        match List.choose tryGetSubscribeAction _channels with
        | [subscribe] ->
            handleRequest
            |> unbox<HandleRequest<'TRequest> -> unit> subscribe
        | _ ->
            x.addChannel <| RequestChannel [ handleRequest ]

    member x.subscribeToResponse<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> (handleResponse : HandleResponse<'TRequest, 'TResponse>) =
        let tryGetSubscribeAction (channel : Channel) =
            channel.getBoxedSubscribeActionIfTypeIs<'TResponse>()
        match List.choose tryGetSubscribeAction _channels with
        | [subscribe] ->
            handleResponse
            |> unbox<HandleResponse<'TRequest, 'TResponse> -> unit> subscribe
        | _ ->
            x.addChannel <| RequestChannel<'TRequest> []
            x.addChannel <| Channel [ handleResponse ]

    interface SubscribeToBus with
        member x.subscribe handle = x.subscribe handle
        member x.subscribeToRequest handleRequest = x.subscribeToRequest handleRequest
        member x.subscribeToResponse handleResponse = x.subscribeToResponse handleResponse
