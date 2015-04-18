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

    let testFile =
        let text = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX\n\
                    X Line #1                                                                      X\n\
                    X Line #2                                                                      X\n\
                    X Line #3                                                                      X\n\
                    X Line #4                                                                      X\n\
                    X Line #5                                                                      X\n\
                    X Line #6                                                                      X\n\
                    X Line #7                                                                      X\n\
                    X Line #8                                                                      X\n\
                    X Line #9                                                                      X\n\
                    X Line #10                                                                     X\n\
                    X Line #11                                                                     X\n\
                    X Line #12                                                                     X\n\
                    X Line #13                                                                     X\n\
                    X Line #14                                                                     X\n\
                    X Line #15                                                                     X\n\
                    X Line #16                                                                     X\n\
                    X Line #17                                                                     X\n\
                    X Line #18                                                                     X\n\
                    X Line #19                                                                     X\n\
                    X Line #20                                                                     X\n\
                    X Line #21                                                                     X\n\
                    X Line #22                                                                     X\n\
                    X Line #23                                                                     X\n\
                    XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
        BufferType.File { emptyFile with Contents = text.Split [|'\n'|] |> Array.toList }

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
