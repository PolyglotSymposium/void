namespace Void.Core

module Buffer =
    open CellGrid

    let emptyFile =
        { Filepath = None; Contents = []; CursorPosition = originCell }

    let newFile path =
        { Filepath = Some path; Contents = []; CursorPosition = originCell }

    let existingFile path contents =
        { Filepath = Some path; Contents = contents; CursorPosition = originCell }

    let readLines fileBuffer start =
        fileBuffer.Contents |> Seq.skip (start - 1) // Line numbers start at 1

type Buffers = private {
    List : Map<int, FileBuffer>
    LastId : int
}

module BufferList =
    let empty = 
        {
            List = Map.empty<int, FileBuffer>
            LastId = 0
        }

    let private newId (bufferList : Buffers) =
        bufferList.LastId + 1

    let private addBuffer bufferList buffer =
        let id = newId bufferList
        let listPlusOne = {
            List = bufferList.List.Add(id, buffer)
            LastId = id
        }
        (listPlusOne, CoreEvent.BufferAdded (id, buffer) :> Message )

    let private addEmptyBuffer bufferList =
        addBuffer bufferList Buffer.emptyFile

    let private writeBufferToPath bufferList bufferId path =
        let lines = Buffer.readLines bufferList.List.[bufferId] 0
        let msg = Filesystem.Command.SaveToDisk (path, lines) :> Message
        (bufferList, msg)

    let private writeBuffer bufferList bufferId = 
        let fileBuffer = bufferList.List.[bufferId]
        match fileBuffer.Filepath with
        | Some path ->
            writeBufferToPath bufferList bufferId path
        | None -> bufferList, noMessage

    let handleEvent bufferList event =
        match event with
        | CoreEvent.FileOpenedForEditing (path, lines) ->
            Buffer.existingFile path (Seq.toList lines)
            |> addBuffer bufferList
        | CoreEvent.NewFileForEditing path ->
            addBuffer bufferList (Buffer.newFile path)
        | _ -> 
            (bufferList, noMessage)

    let handleCommand bufferList command =
        match command with
        | CoreCommand.InitializeVoid ->
            addEmptyBuffer bufferList
        | CoreCommand.WriteBuffer bufferId ->
            writeBuffer bufferList bufferId
        | CoreCommand.WriteBufferToPath (bufferId, path) ->
            writeBufferToPath bufferList bufferId path
        | _ -> 
            (bufferList, noMessage)

    module Service =
        let subscribe (subscribeHandler : SubscribeToBus) =
            let bufferList = ref empty
            subscribeHandler.subscribe <| Service.wrap bufferList handleCommand
            subscribeHandler.subscribe <| Service.wrap bufferList handleEvent
