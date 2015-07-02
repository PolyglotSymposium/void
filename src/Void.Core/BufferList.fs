namespace Void.Core

type Buffers = Map<int, BufferType>

module Buffer =
    open CellGrid

    let emptyFile =
        { Filepath = None; Contents = []; CursorPosition = originCell }

    let empty =
        BufferType.File emptyFile

module BufferList =
    let empty = 
        Map.empty<int, BufferType>

    let newId (bufferList : Buffers) =
        bufferList.Count + 1

    let viewFile bufferList buffer =
        let id = newId bufferList
        let listPlusOne = bufferList.Add(id, buffer)
        (listPlusOne, Event.BufferAdded (id, buffer) :> Message )

    let readLines buffer start =
        match buffer with
        | BufferType.File fileBuffer ->
            fileBuffer.Contents |> Seq.skip (start - 1) // Line numbers start at 1
        | _ -> Seq.empty

    let handleEvent bufferList event =
        match event with
        | Event.FileOpenedForEditing lines ->
            let buffer = BufferType.File { Buffer.emptyFile with Contents = Seq.toList lines }
            viewFile bufferList buffer
        | Event.NewFileForEditing path ->
            sprintf "\"%s\" [New file]" path
            |> UserNotification.Output
            |> Event.NotificationAdded :> Message |> ignore
            (bufferList, noMessage)
        | _ -> 
            (bufferList, noMessage)

    let handleCommand bufferList command =
        match command with
        | Command.InitializeVoid ->
            (bufferList, noMessage)
        | _ -> 
            (bufferList, noMessage)

module BufferListService =
    open Void.Core

    let private eventHandler bufferList =
        Service.wrap bufferList BufferList.handleEvent

    let private commandHandler bufferList =
        Service.wrap bufferList BufferList.handleCommand

    let build() =
        let bufferList = ref BufferList.empty
        (eventHandler bufferList, commandHandler bufferList)
