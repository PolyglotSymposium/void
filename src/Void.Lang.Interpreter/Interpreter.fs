namespace Void.Lang.Interpreter

open Void.Lang.Parser

type VoidScriptCommand = {
    Definition : CommandDefinition
    Execute : CommandArguments -> unit
}

module Interpreter =
    let bootstrapped = true
