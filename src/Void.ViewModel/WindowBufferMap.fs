namespace Void.ViewModel

type WindowWithBuffers = {
    WindowId : int
    CurrentBufferId : int
    AlternateBufferId : int option
}

type BuffersMappedToWindows = {
    CurrentWindowId : int
    Windows : WindowWithBuffers list
}

module WindowBufferMap =
    open Void.Core

    let private firstWindow =
        {
            WindowId = 0
            CurrentBufferId = 1
            AlternateBufferId = None
        }

    let empty =
        {
            CurrentWindowId = 0
            Windows = [firstWindow]
        }

    let handleVMCommand windowBufferMap command =
        match command with
        | VMCommand.Edit fileOrBufferId ->
            match fileOrBufferId with
            | FileOrBufferId.Path path ->
                (windowBufferMap, Command.OpenFile path :> Message)
            | FileOrBufferId.CurrentBuffer ->
                (windowBufferMap, noMessage)
            | FileOrBufferId.AlternateBuffer ->
                (windowBufferMap, noMessage)
            | FileOrBufferId.BufferNumber bufferId ->
                (windowBufferMap, noMessage)
        | VMCommand.Write fileOrBufferId ->
            match fileOrBufferId with
            | FileOrBufferId.Path path ->
                (windowBufferMap, Command.WriteBufferToPath (0, path) :> Message)
            | FileOrBufferId.CurrentBuffer ->
                (windowBufferMap, noMessage)
            | FileOrBufferId.AlternateBuffer ->
                (windowBufferMap, noMessage)
            | FileOrBufferId.BufferNumber bufferId ->
                (windowBufferMap, noMessage)

module WindowBufferMapService =
    open Void.Core

    let private vmCommandHandler windowBufferMap =
        Service.wrap windowBufferMap WindowBufferMap.handleVMCommand

    let build() =
        let windowBufferMap = ref WindowBufferMap.empty
        vmCommandHandler windowBufferMap
