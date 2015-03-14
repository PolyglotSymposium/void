﻿namespace Void.Lang.Parser

[<RequireQualifiedAccess>]
type Filepath = // TODO this is very sketchy right now
    | Number of int // #1, #2 etc
    | Current // %
    | Path of string

type RegexArgs = { // TODO this is very sketchy right now
    Match : string // should be Regex?
    Replace : string option
    Options : char list
}

[<RequireQualifiedAccess>]
type Range = // TODO this is very sketchy right now
    | SingleLocation of int
    | FromTo of int * int
    | All

[<RequireQualifiedAccess>]
type CommandType =
    | Nullary // takes no arguments (other than perhaps a bang)
    | Expression // takes an expression, like ;call, ;echo, ;execute
    | Raw // takes the rest of the line as an unparsed blob, such as ;normal
    | File // like ;write or ;edit, takes a file argument with special meaning for %, #, $HOME, etc
    | Regex // Like ;global and ;substitute -- expect something of the form /<regex>/<regexy thing>/<options> etc

type CommandDefinition = {
    ShortName : string
    FullName : string
    AcceptsBang : bool
    AcceptsRange : bool
    Type : CommandType
}

[<RequireQualifiedAccess>]
type CommandArguments = // TODO don't like the exact parallel with CommandType... need to revisit
    | None
    | Expression of Expression
    | Raw of string
    | File of Filepath
    | Regex of RegexArgs

type LineCommand = {
    Range : Range option
    Name : string
    PassedBang : bool
    Arguments : CommandArguments
}

[<RequireQualifiedAccess>]
type ParseError =
    | Generic
    | UnknownCommand of string

[<RequireQualifiedAccess>]
type LineCommandParse =
    | Failed of ParseError
    | Succeeded of LineCommand

module ParseErrors =
    let message error =
        match error with
        | ParseError.Generic -> "Parse failed"
        | ParseError.UnknownCommand name -> sprintf "Unknown command %s" name
    let generic =
        LineCommandParse.Failed ParseError.Generic
    let unknownCommand name =
        ParseError.UnknownCommand name |> LineCommandParse.Failed

module LineCommands =
    let private parseCommandName line =
        (line, "")
    let parseLine line commandDefinitions =
        if line = "" || commandDefinitions = [] // Why can't the type system do this for me?
        then ParseErrors.generic
        else
            let name, rest = parseCommandName line
            let nameMatches cmdDefinition = 
                cmdDefinition.FullName = name || cmdDefinition.ShortName = name
            match List.tryFind nameMatches commandDefinitions with
            | Some commandDefinition ->
                LineCommandParse.Succeeded {
                    Range = None
                    Name = commandDefinition.FullName
                    PassedBang = false
                    Arguments = CommandArguments.None
                }
            | None -> ParseErrors.unknownCommand name