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

type Interpreter = {
    Commands : ExecutableCommand list
}

module Run =
    let line interpreter lineText =
        match LineCommands.parseLine lineText interpreter.Commands with
        | LineCommandParse.Succeeded parsedCommand ->
            parsedCommand.WrappedArguments (ExecutionEnvironment())
        | LineCommandParse.Failed error ->
            () // TODO
        
