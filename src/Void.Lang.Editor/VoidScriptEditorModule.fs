namespace Void.Lang.Editor

open Void.Lang.Parser
open Void.Lang.Interpreter

module VoidScriptEditorModule =
    let commands = [
        {
            Definition =
                {
                    ShortName = ""
                    FullName = ""
                    AcceptsRange = false
                    Type = CommandType.Nullary
                }
            Execute = fun _ -> ()
        }
    ]

type Class1() = 
    member this.X = "F#"
