namespace Void.Core

type EditorService() =
    member x.handleCommand =
        function
        | CoreCommand.Yank
        | CoreCommand.Put
        | CoreCommand.FormatCurrentLine ->
            notImplemented
        | CoreCommand.Quit
        | CoreCommand.QuitAll
        | CoreCommand.QuitAllWithoutSaving
        | CoreCommand.QuitWithoutSaving ->
             CoreEvent.LastWindowClosed :> Message
        | _ -> noMessage
