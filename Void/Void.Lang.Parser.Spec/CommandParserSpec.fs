namespace Void.Lang.Parser.Spec

open Void.Lang.Parser
open NUnit.Framework
open FsUnit

module CommandStubs =
    let simplestDefinition = {
        ShortName = "simp"
        FullName = "simple"
        AcceptsRange = false
        Type = CommandType.Nullary
    }
    let simplestParsed = {
        Range = None
        Name = "simple"
        Arguments = CommandArguments.None
    }

[<TestFixture>]
type ``Parsing``() = 
    [<Test>]
    member x.``should fail when given an empty string``() =
        LineCommands.parseLine "" [CommandStubs.simplestDefinition]
        |> should equal ParseErrors.generic

    [<Test>]
    member x.``should fail when given no line command objects to look for``() =
        LineCommands.parseLine "unknown" []
        |> should equal (ParseErrors.unknownCommand "unknown")

    [<Test>]
    member x.``should fail on an unknown line command with no range, bang or arguments``() =
        LineCommands.parseLine "unknown" [CommandStubs.simplestDefinition]
        |> should equal (ParseErrors.unknownCommand "unknown")

    [<Test>]
    member x.``should parse long form of a command with no range, bang or arguments``() =
        LineCommands.parseLine "simple" [CommandStubs.simplestDefinition]
        |> should equal (LineCommandParse.Succeeded CommandStubs.simplestParsed)

    [<Test>]
    member x.``should parse short form of a command with no range, bang or arguments``() =
        LineCommands.parseLine "simp" [CommandStubs.simplestDefinition]
        |> should equal (LineCommandParse.Succeeded CommandStubs.simplestParsed)

    [<Test>]
    member x.``should not interpolate between short and long command names for now, until that is determined to be valuable``() =
        LineCommands.parseLine "simpl" [CommandStubs.simplestDefinition]
        |> should equal (ParseErrors.unknownCommand "simpl")
