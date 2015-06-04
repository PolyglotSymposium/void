namespace Void.Core

module CommandLine =
    type Arguments = {
        FilePaths : string list
    }

    let noArgs = { FilePaths = [] }

    type ParseResult =
        | ParseSucceeded of Arguments
        | ParseFailed of string

    let parseArgs =
        function
        | [] -> ParseSucceeded noArgs
        | [filename] -> ParseSucceeded { FilePaths = [filename] }
        | unknown -> sprintf "%A" unknown |> ParseFailed
