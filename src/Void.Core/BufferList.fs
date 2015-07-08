﻿namespace Void.Core

module Buffer =
    open CellGrid

    let emptyFile =
        { Filepath = None; Contents = []; CursorPosition = originCell }

    let newFile path =
        BufferType.File { Filepath = Some path; Contents = []; CursorPosition = originCell }

    let existingFile path contents =
        BufferType.File { Filepath = Some path; Contents = contents; CursorPosition = originCell }

    let empty =
        BufferType.File emptyFile

    let readLines buffer start =
        match buffer with
        | BufferType.File fileBuffer ->
            fileBuffer.Contents |> Seq.skip (start - 1) // Line numbers start at 1
        | _ -> Seq.empty

type Buffers = private {
    List : Map<int, BufferType>
    LastId : int
}

module BufferList =
    let private empty = 
        {
            List = Map.empty<int, BufferType>
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
        (listPlusOne, Event.BufferAdded (id, buffer) :> Message )

    let private addEmptyBuffer bufferList =
        addBuffer bufferList Buffer.empty

    let handleEvent bufferList event =
        match event with
        | Event.FileOpenedForEditing (path, lines) ->
            Buffer.existingFile path (Seq.toList lines)
            |> addBuffer bufferList
        | Event.NewFileForEditing path ->
            // TODO where to trigger this notification?
            //sprintf "\"%s\" [New file]" path
            //|> UserNotification.Output
            //|> Event.NotificationAdded :> Message |> ignore
            addBuffer bufferList (Buffer.newFile path)
        | _ -> 
            (bufferList, noMessage)

    let handleCommand bufferList command =
        match command with
        | Command.InitializeVoid ->
            addEmptyBuffer bufferList
        | _ -> 
            (bufferList, noMessage)

    module Service =
        let private eventHandler bufferList =
            Service.wrap bufferList handleEvent

        let private commandHandler bufferList =
            Service.wrap bufferList handleCommand

        let build() =
            let bufferList = ref empty
            (eventHandler bufferList, commandHandler bufferList)
