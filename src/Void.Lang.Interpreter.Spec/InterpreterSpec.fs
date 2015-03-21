namespace Void.Lang.Interpreter.Spec

open Void.Lang.Interpreter
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Interpreting VoidScript``() = 
    [<Test>]
    member x.``should fail when given an empty string``() =
        Interpreter.bootstrapped |> should be True