namespace Void

open Void.Core
open Void.Lang.Parser
open Void.Lang.Interpreter

type InterpreterWrapperService(interpreter : VoidScriptInterpreter) =
    member x.handleInterpretFragmentRequest (request : InterpretScriptFragmentRequest) =
        match request.Language with
        | "VoidScript" ->
            match Run.fragment interpreter request.Fragment with
            | InterpretScriptFragmentResult.Completed ->
                InterpretScriptFragmentResponse.Completed
            | InterpretScriptFragmentResult.ParseIncomplete ->
                InterpretScriptFragmentResponse.ParseIncomplete
            | InterpretScriptFragmentResult.ParseFailed error ->
                Error.ScriptFragmentParseFailed (ParseErrors.textOf error, request.Fragment)
                |> InterpretScriptFragmentResponse.ParseFailed
            |> Some
        | _ -> None

    member x.subscribe (bus : Bus) =
        bus.subscribeToRequest x.handleInterpretFragmentRequest
