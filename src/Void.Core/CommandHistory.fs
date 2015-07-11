namespace Void.Core

type HistoryOfCommands = {
    Commands : string list
    Index : int
}

module CommandHistory =

    [<RequireQualifiedAccess>]
    type Event =
        | MovedToCommand of string
        | CommandAdded
        interface Message

    let private registerCompletedCommand command history =
        let updatedHistory = {
            Commands = command :: history.Commands
            Index = history.Index + 1
        }

        (updatedHistory, Event.CommandAdded :> Message)

    let private next history =
        history.Index + 1

    let private previous history =
        history.Index - 1
        
    let private move indexModifier history =
        let newIndex = indexModifier history
        let indexIsWithinBounds = 0 <= newIndex && newIndex < history.Commands.Length
        if indexIsWithinBounds
        then
            let newHistory = { history with Index = newIndex }
            (newHistory, Event.MovedToCommand history.Commands.[newIndex] :> Message)
        else (history, noMessage)

    let handleEvent history event =
        match event with
        | CommandMode.Event.CommandCompleted command ->
            registerCompletedCommand command history
        | _ ->
            (history, noMessage)

    let handleCommand history command =
        match command with
        | CommandHistoryCommand.MoveToPreviousCommand ->
            move previous history
        | CommandHistoryCommand.MoveToNextCommand ->
            move next history
        | _ ->
            (history, noMessage)