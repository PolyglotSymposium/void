namespace Void.Core

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
        let bufferProxy = {
            MaybeFilepath = buffer.Filepath
            Contents = Seq.ofList buffer.Contents
        }
        (listPlusOne, { BufferId = id; Message = BufferEvent.Added bufferProxy } :> Message )

    let private addEmptyBuffer bufferList =
        addBuffer bufferList Buffer.emptyFile

    let private writeBufferToPath bufferList bufferId path =
        let lines = Buffer.readLines bufferList.List.[bufferId] 0<mLine>
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
            bufferList, noMessage

    let private package bufferId message =
        {
            BufferId = bufferId
            Message = message
        }

    let handleGetBufferContentsRequest bufferList envelope =
        let buffers = (!bufferList).List
        if buffers.ContainsKey envelope.BufferId
        then
            let buffer = buffers.[envelope.BufferId]
            {
                FirstLineNumber = envelope.Message.StartingAtLine
                RequestedContents =
                    if buffer.Contents.Length*1<mLine> < envelope.Message.StartingAtLine
                    then Seq.empty
                    else Buffer.readLines buffer envelope.Message.StartingAtLine
            }
            |> package envelope.BufferId
            |> Some
        else None

    let packageCursorEvent bufferId event =
        match event with
        | Buffer.CursorEvent.CursorMoved(fromCell, toCell) ->
            {
                BufferId = bufferId
                Message = BufferEvent.CursorMoved(fromCell, toCell)
            } :> Message
        | Buffer.CursorEvent.DidNotMove ->
            noMessage

    let selectBufferAndDelegate handler packageMessage bufferList envelopeMessage =
        let buffer, message = handler bufferList.List.[envelopeMessage.BufferId] envelopeMessage.Message
        let updatedBufferList = { bufferList with List = bufferList.List.Add(envelopeMessage.BufferId, buffer)}
        updatedBufferList, packageMessage envelopeMessage.BufferId message

    module Service =
        let subscribe (bus : Bus) =
            let bufferList = ref empty

            Service.wrap bufferList handleCommand |> bus.subscribe
            Service.wrap bufferList handleEvent |> bus.subscribe

            handleGetBufferContentsRequest bufferList |> bus.subscribeToPackagedRequest

            selectBufferAndDelegate Buffer.handleMoveCursorByRows packageCursorEvent
            |> Service.wrap bufferList
            |> bus.subscribe

            selectBufferAndDelegate Buffer.handleMoveCursorByColumns packageCursorEvent
            |> Service.wrap bufferList
            |> bus.subscribe

            selectBufferAndDelegate Buffer.handleMoveCursorToLineInBuffer packageCursorEvent
            |> Service.wrap bufferList
            |> bus.subscribe
