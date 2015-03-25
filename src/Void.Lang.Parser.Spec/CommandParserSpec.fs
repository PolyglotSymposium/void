namespace Void.Lang.Parser.Spec

open Void.Lang.Parser
open NUnit.Framework
open FsUnit
open System

module CommandStubs =
    let simplestDefinition = {
        ShortName = "simp"
        FullName = "simple"
        WrapArguments = ArgumentWrapper.Nullary (fun () -> true :> Object)
    }
    let simplestParsed = {
        Range = None
        Name = "simple"
        WrappedArguments = true :> Object
    }
    let rawArgumentDefinition = {
        ShortName = "raw"
        FullName = "raw"
        WrapArguments = ArgumentWrapper.Raw (fun actualArgument -> actualArgument :> Object)
    }
    let definitions = [
        simplestDefinition
        rawArgumentDefinition
    ]

[<TestFixture>]
type ``Parsing commands``() =
    [<Test>]
    member x.``should fail when given an empty string``() =
        LineCommands.parseLine "" CommandStubs.definitions
        |> should equal ParseErrors.generic

    [<Test>]
    member x.``should fail when given no line command objects to look for``() =
        LineCommands.parseLine "unknown" []
        |> should equal (ParseErrors.unknownCommand "unknown")

    [<Test>]
    member x.``should fail on an unknown line command with no range, bang or arguments``() =
        LineCommands.parseLine "unknown" CommandStubs.definitions
        |> should equal (ParseErrors.unknownCommand "unknown")

    [<Test>]
    member x.``should parse long form of a command with no range, bang or arguments``() =
        LineCommands.parseLine "simple" CommandStubs.definitions
        |> should equal (LineCommandParse.Succeeded CommandStubs.simplestParsed)

    [<Test>]
    member x.``should parse short form of a command with no range, bang or arguments``() =
        LineCommands.parseLine "simp" CommandStubs.definitions
        |> should equal (LineCommandParse.Succeeded CommandStubs.simplestParsed)

    [<Test>]
    member x.``should not interpolate between short and long command names for now, until that is determined to be valuable``() =
        LineCommands.parseLine "simpl" CommandStubs.definitions
        |> should equal (ParseErrors.unknownCommand "simpl")

    [<Test>]
    member x.``should give an error if there are trailing characters after a nullary command``() =
        LineCommands.parseLine "simple argument" CommandStubs.definitions
        |> should equal (ParseErrors.trailingCharacters "simple")

    [<Test>]
    member x.``should not give an error if there is only trailing whitespace after a nullary command``() =
        LineCommands.parseLine "simple         " CommandStubs.definitions
        |> should equal (LineCommandParse.Succeeded CommandStubs.simplestParsed)

    [<Test>]
    member x.``should parse args for command type of raw``() =
        LineCommands.parseLine "raw $ymb0lz & spacez & #s" CommandStubs.definitions
        |> should equal (LineCommandParse.Succeeded {
            Range = None
            Name = "raw"
            WrappedArguments = " $ymb0lz & spacez & #s" :> Object // TODO technically that space shouldn't be there at the beginning
        })
