namespace Void.Lang.Editor

open Void.Core
open Void.Lang.Parser
open Void.Lang.Interpreter

type VoidScriptEditorModule(publish : Command -> unit) =
    let redraw _ execEnv =
        publish Command.Redraw
    let edit raw execEnv =
        match raw with
        | "%" -> FileIdentifier.CurrentBuffer
        | "#" -> FileIdentifier.AlternateBuffer
        // TODO better parsing, include #2, etc
        | _ -> FileIdentifier.Path raw
        |> Command.Edit 
        |> publish
    let commands = [
        {
            ShortName = "redr"
            FullName = "redraw"
            WrapArguments = CommandType.NoArgs redraw
        }
        {
            ShortName = "e"
            FullName = "edit"
            // TODO Note that in Vim it can take optional ++opt and +cmd args
            WrapArguments = CommandType.Unparsed edit
        }
    ]
