namespace Void.ViewModel

type WindowWithBuffers = {
    CurrentBufferId : int
    AlternateBufferId : int option
}

type BuffersMappedToWindows = {
    CurrentWindowId : int
    Windows : Map<int, WindowWithBuffers>
}

module WindowBufferMap =
    open Void.Core

    let private firstWindow =
        {
            CurrentBufferId = 1
            AlternateBufferId = None
        }

    let empty =
        {
            CurrentWindowId = 0
            Windows = Map.empty.Add(0, firstWindow)
        }

    let private currentWindow windowBufferMap =
        windowBufferMap.Windows.[windowBufferMap.CurrentWindowId]

    let private replaceCurrentWindow windowBufferMap bufferId =
        windowBufferMap.Windows.Add(windowBufferMap.CurrentWindowId, bufferId)

    let private currentBufferId windowBufferMap =
        (currentWindow windowBufferMap).CurrentBufferId

    let private setCurrentBuffer bufferId window =
        {
            CurrentBufferId = bufferId
            AlternateBufferId = Some window.CurrentBufferId
        }

    let private loadBufferIntoCurrentWindow windowBufferMap bufferId =
        let updated = {
            windowBufferMap with Windows = currentWindow windowBufferMap
                                           |> setCurrentBuffer bufferId
                                           |> replaceCurrentWindow windowBufferMap
        }
        (updated, VMEvent.BufferLoadedIntoWindow :> Message)

    let handleVMCommand windowBufferMap command =
        match command with
        | VMCommand.Edit fileOrBufferId ->
            match fileOrBufferId with
            | FileOrBufferId.Path path ->
                (windowBufferMap, FilesystemCommand.OpenFile path :> Message)
            | FileOrBufferId.CurrentBuffer ->
                (windowBufferMap, noMessage)
            | FileOrBufferId.AlternateBuffer ->
                (windowBufferMap, noMessage)
            | FileOrBufferId.BufferNumber bufferId ->
                (windowBufferMap, noMessage)
        | VMCommand.Write fileOrBufferId ->
            match fileOrBufferId with
            | FileOrBufferId.Path path ->
                let id = currentBufferId windowBufferMap
                in (windowBufferMap, Command.WriteBufferToPath (id, path) :> Message)
            | FileOrBufferId.CurrentBuffer ->
                (windowBufferMap, noMessage)
            | FileOrBufferId.AlternateBuffer ->
                (windowBufferMap, noMessage)
            | FileOrBufferId.BufferNumber bufferId ->
                (windowBufferMap, noMessage)

    let handleEvent windowBufferMap event =
        match event with
        | Event.BufferAdded (bufferId, buffer) ->
            loadBufferIntoCurrentWindow windowBufferMap bufferId
        | _ ->
            (windowBufferMap, noMessage)

    module Service =
        open Void.Core

        let private vmCommandHandler windowBufferMap =
            Service.wrap windowBufferMap handleVMCommand

        let build() =
            let windowBufferMap = ref empty
            vmCommandHandler windowBufferMap
