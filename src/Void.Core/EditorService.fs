namespace Void.Core

type EditorService() =
    let mutable _editorState = Editor.defaultState

    member x.handleCommand =
        function
        | Command.Yank
        | Command.Put
        | Command.FormatCurrentLine ->
            notImplemented
        | Command.InitializeVoid ->
            _editorState <- Editor.init CommandLine.noArgs
            Event.EditorInitialized _editorState :> Message
        | Command.Quit
        | Command.QuitAll
        | Command.QuitAllWithoutSaving
        | Command.QuitWithoutSaving ->
             Event.LastWindowClosed :> Message
        | Command.Write fileId ->
            notImplemented
        | _ -> noMessage

    member x.handleEvent =
        function
        | Event.FileOpenedForEditing lines ->
            let buffer = BufferType.File { Buffer.emptyFile with Contents = Seq.toList lines }
            _editorState <- Editor.viewFile _editorState buffer
            // TODO Should services really create the events?...
            Event.BufferLoadedIntoWindow buffer :> Message
        | Event.FileOpenedForViewing lines ->
            let buffer = BufferType.File { Buffer.emptyFile with Contents = Seq.toList lines }
            _editorState <- Editor.viewFile _editorState buffer
            // TODO Should services really create the events?...
            Event.BufferLoadedIntoWindow buffer :> Message
        | Event.NewFileForEditing path ->
            sprintf "\"%s\" [New file]" path
            |> UserNotification.Output
            |> Event.NotificationAdded :> Message
        | Event.NewFileForViewing path ->
            sprintf "\"%s\" [New file]" path
            |> UserNotification.Output
            |> Event.NotificationAdded :> Message
        | _ -> noMessage
