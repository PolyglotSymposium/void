namespace Void.Lang.Interpreter

open Void.Lang.Parser

type ExecutionEnvironment() =
    member x.raise() = 
        ()

type CommandType = ArgumentWrapper<ExecutionEnvironment -> unit>
type ExecutableCommand = CommandDefinition<ExecutionEnvironment -> unit>

module Interpreter =
    let bootstrapped = true
