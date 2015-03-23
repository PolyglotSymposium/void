namespace Void.Core

type MessageController() =
    let mutable _messages = []

    member x.handleEvent event =
        match event with
        | Event.ErrorOccurred error ->
            // TODO ...Or should the underlying model create the events?
            let messages, event = Messages.addError error _messages
            _messages <- messages
            Command.PublishEvent event
        | _ -> Command.Noop

    member x.handleCommand command =
        match command with
        | Command.ShowMessages ->
            Command.Display <| Displayable.Messages _messages
        | _ -> Command.Noop


type EditorController() =
    let mutable _editorState = Editor.defaultState

    member x.handleCommand command =
        match command with
        | Command.Edit
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
        | Command.Quit ->
            Command.PublishEvent Event.LastWindowClosed
        | _ -> Command.Noop
