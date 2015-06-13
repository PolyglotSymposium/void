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
        {
            CurrentBuffer = 0
            BufferList = [Buffer.empty]
        }

    let viewFile editor file =
        { editor with BufferList = [file] }

    let init (commands : CommandLine.Arguments) =
        defaultState

    let currentBuffer editor =
        editor.BufferList.[editor.CurrentBuffer]

    let readLines buffer start =
        match buffer with
        | BufferType.File fileBuffer ->
            fileBuffer.Contents |> Seq.skip (start - 1) // Line numbers start at 1
        | _ -> Seq.empty
