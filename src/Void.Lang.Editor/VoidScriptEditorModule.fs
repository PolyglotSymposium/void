namespace Void.Lang.Editor

open Void.Core
open Void.Lang.Parser
open Void.Lang.Interpreter

type VoidScriptEditorModule(publish : Command -> unit) =
    let redraw _ execEnv =
        publish Command.Redraw
    let commands = [
        {
            ShortName = "redr"
            FullName = "redraw"
            WrapArguments = CommandType.Nullary redraw
        }
    ]
