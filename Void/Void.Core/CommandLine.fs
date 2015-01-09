namespace Void.Core

module CommandLine =
    type Arguments = {
        FilePaths : string list
    }

    type ParseResult =
        | ParseSucceeded of Arguments
        | ParseFailed of string

    let parseArgs (args : string list) =
        match args with
        | [] -> ParseSucceeded { FilePaths = [] }
        | [filename] -> ParseSucceeded { FilePaths = [filename] }
        | unknown -> sprintf "%A" unknown |> ParseFailed