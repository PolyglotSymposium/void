namespace Void.Lang.Parser.Spec

open Void.Lang.Parser
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Parsing``() = 
    [<Test>]
    member x.``should fail when given an empty string``() =
        LineCommands.parseLine "" |> should equal LineCommandParse.Failed
