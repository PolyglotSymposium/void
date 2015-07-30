namespace Void.Core

// TODO This is naive, obviously
type FileBuffer = private {
    Filepath : string option
    Contents : string list
    CursorPosition : CellGrid.Cell
}

module Buffer =
    open CellGrid

    let emptyFile =
        { Filepath = None; Contents = []; CursorPosition = originCell }

    let newFile path =
        { Filepath = Some path; Contents = []; CursorPosition = originCell }

    let existingFile path contents =
        { Filepath = Some path; Contents = contents; CursorPosition = originCell }

    let readLines fileBuffer start =
        fileBuffer.Contents |> Seq.skip ((start - 1<mLine>)/1<mLine>) // Line numbers start at 1

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
            (bufferList, noMessage)

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
            :> EnvelopeMessage<GetBufferContentsResponse>
            //:> EnvelopeMessage<ResponseMessage<GetBufferContentsRequest>>
            |> Some
        else None

    module Service =
        let subscribe (bus : Bus) =
            let bufferList = ref empty
            bus.subscribe <| Service.wrap bufferList handleCommand
            bus.subscribe <| Service.wrap bufferList handleEvent
