namespace Void.Core

type HistoryOfCommands = {
    Commands : string list
    Index : int
}

module CommandHistory =

    let empty = {
        Commands = []
        Index = -1
    }

    let private registerCompletedCommand command history =
        let updatedHistory = {
            history with Commands = command :: history.Commands
        }
        (updatedHistory, CommandHistoryEvent.CommandAdded :> Message)

    let private next history =
        history.Index - 1

    let private previous history =
        history.Index + 1

    let private move indexModifier history =
        let newIndex = indexModifier history
        let indexIsWithinBounds = 0 <= newIndex && newIndex < history.Commands.Length
        let newHistory = { history with Index = newIndex }
        match indexIsWithinBounds, newIndex = -1 with
           | true, false ->
             (newHistory, CommandHistoryEvent.MovedToCommand history.Commands.[newIndex] :> Message)
           | false, true ->
             (newHistory, CommandHistoryEvent.MovedToEmptyCommand :> Message)
           | _ ->
             (history, noMessage)

    let handleEvent history event =
        match event with
        | CommandMode.Event.CommandCompleted command ->
            registerCompletedCommand command history
        | _ ->
            (history, noMessage)

    let handleCoreEvent history event =
        match event with
        | CoreEvent.ErrorOccurred (Error.ScriptFragmentParseFailed (_, command)) ->
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

    module Service =
        let subscribe (subscribeHandler : SubscribeToBus) =
            let history = ref empty
            subscribeHandler.subscribe <| Service.wrap history handleEvent
            subscribeHandler.subscribe <| Service.wrap history handleCommand
            subscribeHandler.subscribe <| Service.wrap history handleCoreEvent