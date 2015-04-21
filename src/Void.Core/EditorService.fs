namespace Void.Core

type EditorService() =
    let mutable _editorState = Editor.defaultState

    member x.handleCommand =
        function
        | Command.Edit _
        | Command.Yank
        | Command.Put
        | Command.FormatCurrentLine ->
            notImplemented
        | Command.ViewTestBuffer ->
            let buffer = Buffer.testFile
            _editorState <- Editor.viewFile _editorState buffer
            // TODO Should controllers create the events?...
            Event.BufferLoadedIntoWindow buffer :> Message
        | Command.InitializeVoid ->
            _editorState <- Editor.init CommandLine.noArgs
            Event.EditorInitialized _editorState :> Message
        | Command.Quit
        | Command.QuitAll
        | Command.QuitAllWithoutSaving
        | Command.QuitWithoutSaving ->
             Event.LastWindowClosed :> Message
        | _ -> noMessage
