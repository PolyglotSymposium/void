namespace Void.Lang.Interpreter

open Void.Lang.Parser

type VoidScriptInterpreter = {
    Commands : ExecutableCommand list
}

[<RequireQualifiedAccess>]
type InterpretFullScriptResult =
    | ParseFailed of ParseError
    | Completed

[<RequireQualifiedAccess>]
type InterpretScriptFragmentResult =
    | ParseFailed of ParseError
    | ParseIncomplete
    | Completed

module Interpreter =
    let instance = { Commands = BuiltinCommands.commands }
    let importCommands interpreter commands = 
        { interpreter with Commands = List.concat [interpreter.Commands; commands] }
    let init commands =
        { Commands = List.concat [instance.Commands; commands] }

module Run =
    let fragment interpreter lineText =
        // This is to run a fragment... be we are only telling the parser to parse a line
        match LineCommands.parseLine lineText interpreter.Commands with
        | LineCommandParse.Incomplete ->
            InterpretScriptFragmentResult.ParseIncomplete
        | LineCommandParse.Succeeded parsedCommand ->
            parsedCommand.WrappedArguments (ExecutionEnvironment())
            InterpretScriptFragmentResult.Completed
        | LineCommandParse.Failed error ->
            InterpretScriptFragmentResult.ParseFailed error
        