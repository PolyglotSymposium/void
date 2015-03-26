namespace Void.Lang.Interpreter

open Void.Lang.Parser

module BuiltinCommands =
    let call expressions execEnv =
        () // TODO
    let beginIf expressions execEnv =
        () // TODO
    let endIf _ execEnv =
        () // TODO
    let echo expressions execEnv =
        () // TODO
    let execute expressions execEnv =
        () // TODO
    let commands = [
        {
            ShortName = "cal"
            FullName = "call"
            WrapArguments = CommandType.Expressions call
        }
        {
            ShortName = "ec"
            FullName = "echo"
            WrapArguments = CommandType.Expressions echo
        }
        {
            ShortName = "en"
            FullName = "endif"
            WrapArguments = CommandType.NoArgs endIf
        }
        {
            ShortName = "exe"
            FullName = "execute"
            WrapArguments = CommandType.Expressions execute
        }
        {
            ShortName = "if"
            FullName = "if"
            WrapArguments = CommandType.Expressions beginIf
        }
    ]
