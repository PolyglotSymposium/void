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

    let private writeLines path lines =
        try
            File.WriteAllLines(path, Seq.toList lines)
            Event.FileSaved path
        with
        | :? UnauthorizedAccessException ->
            Error.AccessToPathNotAuthorized path
            |> Event.FileSaveFailed

    let handleCommand =
        function
        | Command.Edit fileId ->
            match fileId with
            | FileIdentifier.Path path ->
                if File.Exists path
                then
                    match readLines path with
                    | Lines lines ->
                        Event.FileOpenedForEditing lines :> Message
                    | Failure error ->
                        UserNotification.Error error
                        |> Event.NotificationAdded :> Message
                else Event.NewFileForEditing path :> Message
            | _ -> notImplemented
        | Command.SaveToDisk (path, lines) ->
            writeLines path lines :> Message
        | _ -> noMessage