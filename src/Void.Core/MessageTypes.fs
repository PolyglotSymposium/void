namespace Void.Core

[<RequireQualifiedAccess>]
type Error =
    | NotImplemented

module Errors =
    let message error =
        match error with
        | Error.NotImplemented -> "That command is not yet implemented"

[<RequireQualifiedAccess>]
type Message =
    | Output of string
    | Error of Error
