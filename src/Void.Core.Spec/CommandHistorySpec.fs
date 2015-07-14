namespace Void.Core.Spec

open Void.Core
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Command history``() = 
    let empty_history = CommandHistory.empty

    let move_previous history =
        CommandHistory.handleCommand history CommandHistoryCommand.MoveToPreviousCommand

    let move_next history =
        CommandHistory.handleCommand history CommandHistoryCommand.MoveToNextCommand

    [<Test>]
    member x.``does nothing when handling a move previous command with no history``() =
        CommandHistory.handleCommand empty_history CommandHistoryCommand.MoveToPreviousCommand
            |> should equal (empty_history, noMessage)

    [<Test>]
    member x.``moves back a command when a successful command has been registered and handling a move previous``() =
        let historyWithCommand = { empty_history with Commands = ["command"] }

        let (_, eventProduced) = move_previous historyWithCommand
        eventProduced |> should equal <| CommandHistoryEvent.MovedToCommand "command"

    [<Test>]
    member x.``moves back a command when an unsuccessful command has been registered and handling a move previous``() =
        let (newHistory, _) = CommandHistory.handleCoreEvent empty_history <| CoreEvent.ErrorOccurred (Error.ScriptFragmentParseFailed ("...", "command"))
        let (_, eventProduced) = move_previous newHistory
        eventProduced |> should equal <| CommandHistoryEvent.MovedToCommand "command"

    [<Test>]
    member x.``moves back to the empty command a command has been registered and previous then next moves have been made``() =
        let historyWithCommand = { Index = 0; Commands = ["command"] }
        let (_, eventProduced) = move_next historyWithCommand
        eventProduced |> should equal <| CommandHistoryEvent.MovedToEmptyCommand

    [<Test>]
    member x.``moves to last command, when two commands are registered, previous happens twice, then next``() =
        let historyWithCommand = { Index = 1; Commands = ["last_command"; "command"] }
        let (_, eventProduced) = move_next historyWithCommand
        eventProduced |> should equal <| CommandHistoryEvent.MovedToCommand "last_command"
