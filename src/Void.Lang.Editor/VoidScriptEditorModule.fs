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
            WrapArguments = CommandType.Nullary redraw
        }
        {
            ShortName = "e"
            FullName = "edit"
            WrapArguments = CommandType.Raw edit
        }
    ]
