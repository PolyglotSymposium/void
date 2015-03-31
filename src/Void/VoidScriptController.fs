namespace Void

open Void.Core
open Void.Lang.Interpreter

type VoidScriptController(interpreter : VoidScriptInterpreter) =
    member x.handleCommand command =
        match command with
        | Command.InterpretCommandModeCommand line ->
            Run.line interpreter line
        | _ -> ()
        Command.Noop