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
        handlers : ('TIn -> Message) list
    ) =
    let mutable _handlers = handlers

    member x.addHandler handler =
        _handlers <- handler :: _handlers

    interface Channel with
        member x.publish (message : Message) =
            match message with
            | :? 'TIn as msg ->
                Seq.map (fun handle -> handle msg) _handlers
            | _ -> Seq.empty

        member x.getBoxedSubscribeActionIfTypeIs<'TMsg>() =
            if typeof<'TIn> = typeof<'TMsg>
            then Some <| box x.addHandler
            else None

type Bus
    (
        channels : Channel list
    ) =
    let mutable _channels = channels

    member private x.addChannel channel =
        _channels <- channel :: _channels

    member x.publishAll messages =
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
            |> unbox<('TMsg -> Message) -> unit> subscribe
        | _ ->
            x.addChannel <| Channel [ handle ]

    interface SubscribeToBus with
        member x.subscribe handle = x.subscribe handle
