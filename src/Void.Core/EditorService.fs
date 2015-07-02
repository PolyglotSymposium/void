namespace Void.Core

type EditorService() =
    member x.handleCommand =
        function
        | Command.Yank
        | Command.Put
        | Command.FormatCurrentLine ->
            notImplemented
        | Command.Quit
        | Command.QuitAll
        | Command.QuitAllWithoutSaving
        | Command.QuitWithoutSaving ->
             Event.LastWindowClosed :> Message
        | _ -> noMessage
