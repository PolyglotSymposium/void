﻿namespace Void.ViewModel

open Void.Core

[<RequireQualifiedAccess>]
type StatusLineView = // TODO much yet to be done here
    | Unfocused
    | Focused

type WindowView = {
    StatusLine : StatusLineView
    Dimensions : CellGrid.Dimensions
    Buffer : string list
    Cursor : CursorView Visibility
    TopLineNumber : int<mLine>
}

module Window =
    open Void.Util
    open Void.Core.CellGrid

    [<RequireQualifiedAccess>]
    type Event =
        | ContentsUpdated of WindowView
        | Initialized of WindowView
        interface EventMessage

    [<RequireQualifiedAccess>]
    type Command =
        | RedrawWindow of WindowView
        interface CommandMessage

    let private zeroWindowView =
        {
            StatusLine = StatusLineView.Focused
            Buffer = []
            Dimensions = zeroDimensions
            Cursor = Visible <| CursorView.Block originCell
            TopLineNumber = 1<mLine>
        }

    let private windowInArea window containingArea =
        { zeroWindowView with Dimensions = (lessRowsBelow 1 containingArea).Dimensions }

    let defaultWindowView =
        { zeroWindowView with Dimensions = Sizing.defaultViewSize }

    let bufferFrom (windowSize : Dimensions) lines =
        let truncateToWindowWidth = StringUtil.noLongerThan windowSize.Columns
        lines
        |> SeqUtil.notMoreThan windowSize.Rows
        |> Seq.map truncateToWindowWidth
        |> Seq.toList

    let private toScreenBuffer windowSize (buffer : FileBufferProxy) =
        bufferFrom windowSize buffer.Contents

    let private loadBufferIntoWindow buffer (window : WindowView) =
        let updatedWindow = { window with Buffer = toScreenBuffer window.Dimensions buffer }
        updatedWindow, Event.ContentsUpdated updatedWindow :> Message

    let handleBufferEvent window event =
        match event.Message with
        | BufferEvent.Added buffer ->
            loadBufferIntoWindow buffer window

    let private scroll (requestSender : RequestSender) window xLines =
        let request : GetWindowContentsRequest = { StartingAtLine = window.TopLineNumber + xLines }
        match requestSender.makeRequest request with
        | Some (response : GetWindowContentsResponse) ->
            let updatedWindow = { window with TopLineNumber = response.FirstLineNumber; Buffer = Seq.toList response.RequestedContents }
            updatedWindow, Event.ContentsUpdated updatedWindow :> Message
        | None -> window, noMessage

    let handleVMCommand requestSender window command =
        match command with
        | VMCommand.Scroll movement ->
            match movement with
            | Move.Backward xLines ->
                // TODO bounds checking
                scroll requestSender window -xLines
            | Move.Forward xLines ->
                scroll requestSender window xLines
        | _ ->
            window, noMessage

    let handleCoreCommand window command =
        match command with
        | CoreCommand.Redraw ->
            Command.RedrawWindow !window :> Message
        | _ -> noMessage

    let handleVMEvent window event =
        match event with
        | VMEvent.ViewModelInitialized viewModel ->
            let updatedWindow = windowInArea window (ViewModel.wholeArea viewModel)
            updatedWindow, Event.Initialized updatedWindow :> Message
        | _ -> window, noMessage

    module Service =
        let subscribe (bus : Bus) =
            let window = ref zeroWindowView
            Service.wrap window handleBufferEvent |> bus.subscribe
            handleCoreCommand window |> bus.subscribe
            Service.wrap window handleVMEvent |> bus.subscribe
            Service.wrap window (handleVMCommand bus) |> bus.subscribe
