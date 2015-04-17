namespace Void.Core

type EditorService() =
    let mutable _editorState = Editor.defaultState

    member x.handleCommand command =
        match command with
        | Command.Edit _
        | Command.Yank
        | Command.Put
        | Command.FormatCurrentLine ->
            notImplemented 
        | Command.ViewTestBuffer ->
            let buffer = Buffer.testFile
            _editorState <- Editor.viewFile _editorState buffer
            // TODO Should controllers create the events?...
            Event.BufferLoadedIntoWindow buffer |> Command.PublishEvent
        | Command.InitializeVoid ->
            _editorState <- Editor.init CommandLine.noArgs
            Command.PublishEvent <| Event.EditorInitialized _editorState
        | Command.Quit
        | Command.QuitAll
        | Command.QuitAllWithoutSaving
        | Command.QuitWithoutSaving ->
            Command.PublishEvent Event.LastWindowClosed
        | _ -> Command.Noop
