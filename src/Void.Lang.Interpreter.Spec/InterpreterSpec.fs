namespace Void.Lang.Interpreter.Spec

open Void.Lang.Interpreter
open Void.Lang.Parser
open NUnit.Framework
open FsUnit


[<TestFixture>]
type ``VoidScript interpreter``() = 
    let commandName = "foo"
    let setup func =
        let simplestCommand = {
            ShortName = commandName
            FullName = commandName
            WrapArguments = ArgumentWrapper.NoArgs func
        }
        {
            Commands = [simplestCommand]
        }
    [<Test>]
    member x.``should be able to run a simple command``() =
        let invoked = ref false
        let interpreter = setup (fun _ _ -> invoked := true)
        let result = Run.fragment interpreter commandName
        !invoked |> should be True
        result |> should equal InterpretScriptFragmentResult.Completed