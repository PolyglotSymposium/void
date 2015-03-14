namespace Void.Lang.Parser.Spec

open Void.Lang.Parser
open NUnit.Framework
open FsUnit

module Stubs =
    let simplestCommand = {
        Range = None
        Name = "simple"
        PassedBang = false
        Arguments = CommandArguments.Nullary
    }

[<TestFixture>]
type ``Parsing``() = 
    [<Test>]
    member x.``should fail when given an empty string``() =
        LineCommands.parseLine "" [Stubs.simplestCommand] |> should equal ParseErrors.generic

    [<Test>]
    member x.``should fail when given no line command objects to look for``() =
        LineCommands.parseLine "unknown" [] |> should equal ParseErrors.generic

    [<Test>]
    member x.``should fail on an unknown line command with no range, bang or arguments``() =
        LineCommands.parseLine "unknown" [Stubs.simplestCommand] |> should equal (ParseErrors.unknownCommand "unknown")