namespace Void.Lang.Editor

open Void.Core
open Void.Lang.Parser
open Void.Lang.Interpreter

type VoidScriptEditorModule(publish : Command -> unit) =
    let edit raw execEnv =
        match raw with
        | "%" -> FileIdentifier.CurrentBuffer
        | "#" -> FileIdentifier.AlternateBuffer
        // TODO better parsing, include #2, etc
        | _ -> FileIdentifier.Path raw
        |> Command.Edit 
        |> publish

    let messages _ execEnv =
        publish Command.ShowNotificationHistory

    let quit _ execEnv =
        publish Command.Quit

    let quitAll _ execEnv =
        publish Command.QuitAll

    let redraw _ execEnv =
        publish Command.Redraw

    member x.Commands = [
        {
            ShortName = "e"
            FullName = "edit"
            // TODO Note that in Vim it can take optional ++opt and +cmd args
            WrapArguments = CommandType.Unparsed edit
        }
        {
            ShortName = "mes"
            FullName = "messages"
            WrapArguments = CommandType.NoArgs messages
        }
        {
            ShortName = "q"
            FullName = "quit"
            WrapArguments = CommandType.NoArgs quit
        }
        {
            ShortName = "qa"
            FullName = "qall"
            WrapArguments = CommandType.NoArgs quitAll
        }
        {
            ShortName = "redr"
            FullName = "redraw"
            WrapArguments = CommandType.NoArgs redraw
        }
    ]
