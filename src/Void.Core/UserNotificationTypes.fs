namespace Void.Core

[<RequireQualifiedAccess>]
type Error =
    | AccessToPathNotAuthorized of string
    | NoInterpreter
    | NotImplemented
    | ScriptFragmentParseFailed of string * string

module Errors =
    let textOf =
        function
        | Error.AccessToPathNotAuthorized path -> sprintf "Access to path not authorized: \"%s\"" path
        | Error.NoInterpreter -> "No interpreter responded to the request to interpret the command"
        | Error.NotImplemented -> "That command is not yet implemented"
        | Error.ScriptFragmentParseFailed (msg, _) -> msg

[<RequireQualifiedAccess>]
// In Vim this is called a message, e.g. :messages or :echomsg.
// However, to prevent confusion, in Void this is NOT being called a message
// because that has an architectural/infrastructure meaning as well
// which we don't want to get confused with.
type UserNotification =
    | Output of string
    | Error of Error
