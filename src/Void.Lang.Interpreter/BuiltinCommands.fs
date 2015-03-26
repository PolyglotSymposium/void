namespace Void.Lang.Interpreter

open Void.Lang.Parser

module BuiltinCommands =
    let call expressions execEnv =
        () // TODO
    let beginIf expressions execEnv =
        () // TODO
    let endIf _ execEnv =
        () // TODO
    let commands = [
        {
            ShortName = "cal"
            FullName = "call"
            WrapArguments = CommandType.Expression call
        }
        {
            ShortName = "if"
            FullName = "if"
            WrapArguments = CommandType.Expression beginIf
        }
        {
            ShortName = "en"
            FullName = "endif"
            WrapArguments = CommandType.Nullary endIf
        }
    ]
