namespace Void

open Void.Core
open Void.ViewModel

type Channel<'TIn when 'TIn :> Message>
    (
        handlers : ('TIn -> Message) list
    ) =
    let mutable _handlers = handlers

    member x.publish (message : Message) =
        match message with
        | :? 'TIn as msg ->
            Seq.map (fun handle -> handle msg) _handlers
        | _ -> Seq.empty

    member x.addHandler handler =
        _handlers <- handler :: _handlers

type Bus
    (
        channels : (Message -> Message seq) list
    ) =
    let mutable _channels = channels

    member x.addChannel channel =
        _channels <- channel :: _channels

    member x.publishAll messages =
        for message in messages do
            x.publish message

    member x.publish (message : Message) =
        for publishToChannel in _channels do
            publishToChannel message |> x.publishAll
