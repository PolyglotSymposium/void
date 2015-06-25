namespace Void.Core

module Notifications =
    let private addNotification notification notifications = 
        (notification :: notifications), Event.NotificationAdded notification
    let addError error notifications =
        let notification = UserNotification.Error error
        in addNotification notification notifications
    let addOutput text notifications =
        let notification = UserNotification.Output text
        in addNotification notification notifications

module Buffer =
    open CellGrid

    let emptyFile =
        { Filepath = None; Contents = []; CursorPosition = originCell }

    let empty =
        BufferType.File emptyFile

module Editor = 
    let defaultState =
        Map.empty.Add(0, Buffer.empty)

    let viewFile (bufferList : BufferList) buffer =
        // TODO replace instead of add if current buffer is strictly empty
        bufferList.Add(bufferList.Count+1, buffer)

    let readLines buffer start =
        match buffer with
        | BufferType.File fileBuffer ->
            fileBuffer.Contents |> Seq.skip (start - 1) // Line numbers start at 1
        | _ -> Seq.empty
