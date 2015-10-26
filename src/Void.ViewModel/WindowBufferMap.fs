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

    let private getWindowId windowBufferMap bufferId =
        let windowIdOfMatchingBufferId windowId window =
            if window.CurrentBufferId = bufferId
            then Some windowId
            else None
        Map.pick windowIdOfMatchingBufferId windowBufferMap.Windows

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
        updated, VMEvent.BufferLoadedIntoWindow :> Message

    let handleVMCommand windowBufferMap command =
        match command with
        | VMCommand.Edit fileOrBufferId ->
            match fileOrBufferId with
            | FileOrBufferId.Path path ->
                windowBufferMap, Filesystem.Command.OpenFile path :> Message
            | FileOrBufferId.CurrentBuffer ->
                windowBufferMap, noMessage
            | FileOrBufferId.AlternateBuffer ->
                windowBufferMap, noMessage
            | FileOrBufferId.BufferNumber bufferId ->
                windowBufferMap, noMessage
        | VMCommand.Write fileOrBufferId ->
            match fileOrBufferId with
            | FileOrBufferId.Path path ->
                let id = currentBufferId windowBufferMap
                in (windowBufferMap, CoreCommand.WriteBufferToPath (id, path) :> Message)
            | FileOrBufferId.CurrentBuffer ->
                windowBufferMap, noMessage
            | FileOrBufferId.AlternateBuffer ->
                windowBufferMap, noMessage
            | FileOrBufferId.BufferNumber bufferId ->
                windowBufferMap, noMessage
        | _ ->
            windowBufferMap, noMessage

    let handleBufferEvent windowBufferMap event =
        match event.Message with
        | BufferEvent.Added _ ->
            loadBufferIntoCurrentWindow windowBufferMap event.BufferId
        | unwrappedEvent ->
            windowBufferMap, {
                WindowId = getWindowId windowBufferMap event.BufferId
                Message = unwrappedEvent
            } :> Message

    let handleCurrentBufferCommandEnvelope<'TBufferCommand when 'TBufferCommand :> BufferMessage> windowBufferMap (InCurrentBuffer (bufferCommand : 'TBufferCommand)) =
        {
            BufferId = currentBufferId !windowBufferMap
            Message = bufferCommand
        } :> Message

    let getWindowContentsResponse getBufferContentsResponse =
        {
            FirstLineNumber = getBufferContentsResponse.Message.FirstLineNumber
            RequestedContents = getBufferContentsResponse.Message.RequestedContents
        } : GetWindowContentsResponse

    let handleGetWindowContentsRequest (requestSender : PackagedRequestSender) windowBufferMap (request : GetWindowContentsRequest) =
        requestSender.makePackagedRequest {
            BufferId = currentBufferId !windowBufferMap
            Message = { StartingAtLine = request.StartingAtLine }
        }
        |> Option.map getWindowContentsResponse

    module Service =
        open Void.Core

        let subscribe (bus : Bus) =
            let windowBufferMap = ref empty
            Service.wrap windowBufferMap handleVMCommand
            |> bus.subscribe
            Service.wrap windowBufferMap handleBufferEvent
            |> bus.subscribe

            handleGetWindowContentsRequest bus windowBufferMap
            |> bus.subscribeToRequest

            handleCurrentBufferCommandEnvelope<MoveCursor<By.Row>> windowBufferMap
            |> bus.subscribe

            handleCurrentBufferCommandEnvelope<MoveCursorTo<By.Line,In.Buffer>> windowBufferMap
            |> bus.subscribe
