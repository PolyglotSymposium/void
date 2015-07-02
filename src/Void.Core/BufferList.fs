namespace Void.Core

type Buffers = Map<int, BufferType>

module BufferList =
    let empty = 
        Map.empty<int, BufferType>

    let handleEvent bufferList event =
        match event with
        | _ -> 
            (bufferList, noMessage)

module BufferListService =
    open Void.Core

    let private eventHandler bufferList =
        Service.wrap bufferList BufferList.handleEvent

    let build() =
        let bufferList = ref BufferList.empty
        eventHandler bufferList

