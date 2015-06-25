namespace Void.Core

module Filesystem =
    open System
    open System.IO

    type LinesOrFailure =
        | Lines of string seq
        | Failure of Error

    let private readLines path =
        try
            File.ReadLines path |> Lines
        with
        | :? UnauthorizedAccessException ->
            Error.AccessToPathNotAuthorized path |> Failure

    let handleCommand =
        function
        | Command.OpenFile path ->
            if File.Exists path
            then
                match readLines path with
                | Lines lines ->
                    Event.FileOpenedForEditing lines :> Message
                | Failure error ->
                    UserNotification.Error error
                    |> Event.NotificationAdded :> Message
            else Event.NewFileForEditing path :> Message
        | _ -> noMessage