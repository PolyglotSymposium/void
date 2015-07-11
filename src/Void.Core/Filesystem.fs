namespace Void.Core

[<RequireQualifiedAccess>]
type FilesystemCommand =
    | OpenFile of string
    | SaveToDisk of string * string seq
    interface Command

module Filesystem =
    open System
    open System.IO
    open System.Text

    type LinesOrFailure =
        | Lines of string seq
        | Failure of Error

    let private readLines path =
        try
            File.ReadLines(path, Encoding.UTF8) |> Lines
        with
        | :? UnauthorizedAccessException ->
            Error.AccessToPathNotAuthorized path |> Failure

    let private writeLines path lines =
        try
            File.WriteAllLines(path, Seq.toList lines, Encoding.UTF8)
            CoreEvent.FileSaved path
        with
        | :? UnauthorizedAccessException ->
            Error.AccessToPathNotAuthorized path
            |> CoreEvent.ErrorOccurred

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
        | FilesystemCommand.OpenFile path ->
            let path = expandPath path
            if File.Exists path
            then
                match readLines path with
                | Lines lines ->
                    CoreEvent.FileOpenedForEditing (path, lines) :> Message
                | Failure error ->
                    UserNotification.Error error
                    |> CoreEvent.NotificationAdded :> Message
            else CoreEvent.NewFileForEditing path :> Message
        | FilesystemCommand.SaveToDisk (path, lines) ->
            writeLines path lines :> Message
        | _ -> noMessage
