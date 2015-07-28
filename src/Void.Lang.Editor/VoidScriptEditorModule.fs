namespace Void.Lang.Editor

open Void.Core
open Void.Core.CommandLanguage
open Void.Lang.Parser
open Void.Lang.Interpreter
open Void.ViewModel

module VoidScriptEditor =
    let parseFilePath path =
        match path with
        | "%" -> FileOrBufferId.CurrentBuffer
        | "#" -> FileOrBufferId.AlternateBuffer
        // TODO better parsing, include #2, etc
        | _ -> FileOrBufferId.Path path

type VoidScriptEditorModule(publish : Message -> unit) =
    let echo _ execEnv =
        publish <| CoreCommand.Echo ""

    let edit raw execEnv =
        VoidScriptEditor.parseFilePath raw
        |> VMCommand.Edit 
        |> publish

    let help _ execEnv =
        publish CoreCommand.Help

    let messages _ execEnv =
        publish CoreCommand.ShowNotificationHistory

    let python _ execEnv =
        publish <| ChangeCurrentCommandLanguageTo "python2"
    // TODO these really should be registered by the plugin that will provide
    // the corresponding Python interpreter
    let python3 _ execEnv =
        publish <| ChangeCurrentCommandLanguageTo "python3"

    let quit _ execEnv =
        publish CoreCommand.Quit

    let quitAll _ execEnv =
        publish CoreCommand.QuitAll

    let redraw _ execEnv =
        publish CoreCommand.Redraw

    let view raw execEnv =
        edit raw execEnv
        CoreCommand.SetBufferOption EditorOption.ReadOnly
        |> publish

    let write raw execEnv =
        VoidScriptEditor.parseFilePath raw
        |> VMCommand.Write
        |> publish

    member x.Commands = [
        {
            ShortName = "ec"
            FullName = "echo"
            // TODO of course echo really should take expressions
            WrapArguments = CommandType.NoArgs echo
        }
        {
            ShortName = "e"
            FullName = "edit"
            // TODO Note that in Vim it can take optional ++opt and +cmd args
            WrapArguments = CommandType.Unparsed edit
        }
        {
            ShortName = "h"
            FullName = "help"
            // TODO of course echo really should be able to take arguments
            WrapArguments = CommandType.NoArgs help
        }
        {
            ShortName = "mes"
            FullName = "messages"
            WrapArguments = CommandType.NoArgs messages
        }
        {
            ShortName = "py"
            FullName = "python"
            WrapArguments = CommandType.NoArgs python
        }
        {
            ShortName = "py3"
            FullName = "python3"
            WrapArguments = CommandType.NoArgs python3
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
        {
            ShortName = "vie"
            FullName = "view"
            // TODO Note that in Vim it can take optional ++opt and +cmd args
            WrapArguments = CommandType.Unparsed view
        }
        {
            ShortName = "w"
            FullName = "write"
            WrapArguments = CommandType.Unparsed write
        }
    ]
