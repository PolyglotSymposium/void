namespace Void.Core

[<RequireQualifiedAccess>]
type Error =
    | NotImplemented
    | ScriptFragmentParseFailed of string

module Errors =
    let textOf =
        function
        | Error.NotImplemented -> "That command is not yet implemented"
        | Error.ScriptFragmentParseFailed msg -> msg

[<RequireQualifiedAccess>]
// In Vim this is called a message, e.g. :messages or :echomsg.
// However, to prevent confusion, in Void this is NOT being called a message
// because that has an architectural/infrastructure meaning as well
// which we don't want to get confused with.
type UserNotification =
    | Output of string
    | Error of Error
