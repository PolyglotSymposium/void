namespace Void.Lang.Parser

open System

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
type ArgumentWrapper<'a> =
    | NoArgs of (unit -> 'a) // takes no arguments (other than perhaps a bang)
    | Expressions of (Expression list -> 'a) // takes an expression, like ;call, ;echo, ;execute
    | Unparsed of (string -> 'a) // takes the rest of the line as an unparsed blob, such as ;normal
    | Regex of (RegexArgs -> 'a) // Like ;global and ;substitute -- expect something of the form /<regex>/<regexy thing>/<options> etc

type CommandDefinition<'TArgWrapper> = {
    ShortName : string
    FullName : string
    WrapArguments : ArgumentWrapper<'TArgWrapper>
}

type ParsedCommand<'TArgWrapper> = {
    Range : Range option
    Name : string
    WrappedArguments : 'TArgWrapper
}

[<RequireQualifiedAccess>]
type ParseError =
    | Generic
    | UnknownCommand of string // In Vim: E492: Not an editor command: xyz
    | TrailingCharacters of string // In Vim: E488: Trailing characters

[<RequireQualifiedAccess>]
type LineCommandParse<'TArgWrapper> =
    | Failed of ParseError
    | Incomplete // For multiline commands such as ;if and ;for
    | Succeeded of ParsedCommand<'TArgWrapper>

module ParseErrors =
    let textOf =
        function
        | ParseError.Generic -> "Parse failed"
        | ParseError.UnknownCommand name -> sprintf "Unknown command %s" name
        | ParseError.TrailingCharacters name -> sprintf "Trailing characters after command %s" name
    let generic =
        LineCommandParse.Failed ParseError.Generic
    let unknownCommand name =
        ParseError.UnknownCommand name |> LineCommandParse.Failed
    let trailingCharacters name =
        ParseError.TrailingCharacters name |> LineCommandParse.Failed

module LineCommands =
    let private isCommandDeliminator =
        function
        | ' ' -> true
        | _ -> false

    let private parseCommandName line =
        let rec parse parsed rest =
            if rest = "" || isCommandDeliminator rest.[0]
            then (parsed, rest)
            else parse (sprintf "%s%c" parsed rest.[0]) rest.[1..]
        in parse "" line

    let private parseArguments commandDefinition restOfLine =
        match commandDefinition.WrapArguments with
        | ArgumentWrapper.Unparsed wrap ->
            LineCommandParse.Succeeded {
                Range = None
                Name = commandDefinition.FullName
                WrappedArguments = wrap restOfLine 
            }
        | ArgumentWrapper.NoArgs wrap ->
            if System.String.IsNullOrWhiteSpace restOfLine
            then
                LineCommandParse.Succeeded {
                    Range = None
                    Name = commandDefinition.FullName
                    WrappedArguments = wrap()
                }
            else ParseErrors.trailingCharacters commandDefinition.FullName
        | _ -> ParseErrors.generic

    let parseLine line commandDefinitions =
        if line = ""
        then ParseErrors.generic
        else
            let name, rest = parseCommandName line
            let nameMatches cmdDefinition = 
                cmdDefinition.FullName = name || cmdDefinition.ShortName = name
            match List.tryFind nameMatches commandDefinitions with
            | Some commandDefinition -> parseArguments commandDefinition rest
            | None -> ParseErrors.unknownCommand name