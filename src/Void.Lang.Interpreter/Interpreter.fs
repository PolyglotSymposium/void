namespace Void.Lang.Interpreter

open Void.Lang.Parser

type Host =
    abstract member Echo : string -> unit
    abstract member HandleError : string -> unit

type ExecutionEnvironment() =
    member x.raise() = 
        ()

type CommandType = ArgumentWrapper<ExecutionEnvironment -> unit>
type ExecutableCommand = CommandDefinition<ExecutionEnvironment -> unit>

type VoidScriptInterpreter = {
    Commands : ExecutableCommand list
}

module Interpreter =
    let empty = { Commands = [] }
    let importCommands interpreter commands = 
        { interpreter with Commands = List.concat [interpreter.Commands; commands] }
    let init commands =
        { Commands = commands }

module Run =
    let line interpreter lineText =
        match LineCommands.parseLine lineText interpreter.Commands with
        | LineCommandParse.Succeeded parsedCommand ->
            parsedCommand.WrappedArguments (ExecutionEnvironment())
        | LineCommandParse.Failed error ->
            () // TODO
        