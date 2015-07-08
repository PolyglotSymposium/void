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

    let private home() =
        if Environment.OSVersion.Platform = PlatformID.Unix || Environment.OSVersion.Platform = PlatformID.MacOSX
        then Environment.GetEnvironmentVariable("HOME")
        else Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%")

    let private expandPath (path : string) =
        if path.[0] = '~'
        then
            sprintf "%s%s" (home()) path.[1..]
        else path

    let handleCommand =
        function
        | Command.OpenFile path ->
            let path = expandPath path
            if File.Exists path
            then
                match readLines path with
                | Lines lines ->
                    Event.FileOpenedForEditing (path, lines) :> Message
                | Failure error ->
                    UserNotification.Error error
                    |> Event.NotificationAdded :> Message
            else Event.NewFileForEditing path :> Message
        | Command.SaveToDisk (path, lines) ->
            writeLines path lines :> Message
        | _ -> noMessage
