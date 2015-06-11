namespace Void.Core

module Filesystem =
    open System.IO

    let handleCommand =
        function
        | Command.View fileId ->
            match fileId with
            | FileIdentifier.Path path ->
                File.ReadLines path
                |> Event.FileOpenedForViewing :> Message
            | _ -> notImplemented
        | Command.Edit fileId ->
            match fileId with
            | FileIdentifier.Path path ->
                File.ReadLines path
                |> Event.FileOpenedForEditing :> Message
            | _ -> notImplemented
        | _ -> noMessage