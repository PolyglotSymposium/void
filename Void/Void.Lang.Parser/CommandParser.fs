namespace Void.Lang.Parser

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
type CommandArguments =
    | Nullary // takes no arguments (other than perhaps a bang)
    | Expression of Expression // takes an expression, like ;call, ;echo, ;execute
    | Raw of string // takes the rest of the line as an unparsed blob, such as ;normal
    | File of Filepath // like ;write or ;edit, takes a file argument with special meaning for %, #, $HOME, etc
    | Regex of RegexArgs // Like ;global and ;substitute -- expect something of the form /<regex>/<regexy thing>/<options> etc

type LineCommand = {
    Range : Range option
    Name : string
    PassedBang : bool
    Arguments : CommandArguments
}

[<RequireQualifiedAccess>]
type LineCommandParse =
    | Failed
    | Success of LineCommand

module LineCommands =
    let parseLine line =
        LineCommandParse.Failed