namespace Void.Core

[<RequireQualifiedAccess>]
type Error =
    | NotImplemented
    | LineCommandFailed of string

module Errors =
    let message error =
        match error with
        | Error.NotImplemented -> "That command is not yet implemented"
        | Error.LineCommandFailed msg -> msg

[<RequireQualifiedAccess>]
type Message =
    | Output of string
    | Error of Error
