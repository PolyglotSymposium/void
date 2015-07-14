namespace Void.Core.Spec

open Void.Core
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Command history``() = 
    let empty_history = CommandHistory.empty
    let register_command name =
        CommandHistory.handleEvent empty_history <| CommandMode.Event.CommandCompleted name

    [<Test>]
    member x.``does nothing when handling a move previous command with no history``() =
        CommandHistory.handleCommand empty_history CommandHistoryCommand.MoveToPreviousCommand
            |> should equal (empty_history, noMessage)

    [<Test>]
    member x.``moves back a command when a successful command has been registered and handling a move previous``() =
        let (newHistory, _) = register_command "command"
        let (_, eventProduced) = CommandHistory.handleCommand newHistory CommandHistoryCommand.MoveToPreviousCommand
        eventProduced |> should equal <| CommandHistoryEvent.MovedToCommand "command"

    [<Test>]
    member x.``moves back a command when an unsuccessful command has been registered and handling a move previous``() =
        let (newHistory, _) = CommandHistory.handleCoreEvent empty_history <| CoreEvent.ErrorOccurred (Error.ScriptFragmentParseFailed ("...", "command"))
        let (_, eventProduced) = CommandHistory.handleCommand newHistory CommandHistoryCommand.MoveToPreviousCommand
        eventProduced |> should equal <| CommandHistoryEvent.MovedToCommand "command"

    [<Test>]
    member x.``moves back to the empty command a command has been registered and previous then next moves have been made``() =
        let (newHistory, _) = register_command "command"
        let (newHistory2, _) = CommandHistory.handleCommand newHistory CommandHistoryCommand.MoveToPreviousCommand
        let (_, eventProduced) = CommandHistory.handleCommand newHistory2 CommandHistoryCommand.MoveToNextCommand
        eventProduced |> should equal <| CommandHistoryEvent.MovedToEmptyCommand

    [<Test>]
    member x.``moves to last command, when two commands are registered, previous happens twice, then next``() =
        let (newHistory, _) = register_command "command"
        let (newHistory2, _) = CommandHistory.handleEvent newHistory <| CommandMode.Event.CommandCompleted "last_command"
        let (newHistory3, _) = CommandHistory.handleCommand newHistory2 <| CommandHistoryCommand.MoveToPreviousCommand
        let (newHistory4, _) = CommandHistory.handleCommand newHistory3 <| CommandHistoryCommand.MoveToPreviousCommand
        let (_, eventProduced) = CommandHistory.handleCommand newHistory4 <| CommandHistoryCommand.MoveToNextCommand
        eventProduced |> should equal <| CommandHistoryEvent.MovedToCommand "last_command"
