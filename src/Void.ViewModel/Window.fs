﻿namespace Void.ViewModel

open Void.Core

[<RequireQualifiedAccess>]
type StatusLineView = // TODO much yet to be done here
    | Unfocused
    | Focused

type WindowView = {
    StatusLine : StatusLineView
    Area : CellGrid.Block
    Buffer : string list
    Cursor : CursorView Visibility
    TopLineNumber : int<mLine>
}

module Window =
    open Void.Util
    open Void.Core.CellGrid

    [<RequireQualifiedAccess>]
    type Event =
        | ContentsUpdated of string list
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
            Area = zeroBlock
            Cursor = Visible <| CursorView.Block originCell
            TopLineNumber = 1<mLine>
        }

    let private windowInArea window containingArea =
        { zeroWindowView with Area = lessRowsBelow 1 containingArea }

    let bufferFrom (windowSize : Dimensions) lines =
        let truncateToWindowWidth = StringUtil.noLongerThan windowSize.Columns
        lines
        |> SeqUtil.notMoreThan windowSize.Rows
        |> Seq.map truncateToWindowWidth
        |> Seq.toList

    let private toScreenBuffer windowSize (buffer : FileBufferProxy) =
        bufferFrom windowSize buffer.Contents

    let private loadBufferIntoWindow buffer window =
        let updatedContents = toScreenBuffer window.Area.Dimensions buffer
        { window with Buffer = updatedContents }, Event.ContentsUpdated updatedContents :> Message

    let handleBufferEvent window event =
        match event.Message with
        | BufferEvent.Added buffer ->
            loadBufferIntoWindow buffer window

    let handleVMCommand =
        function
        | VMCommand.Scroll movement ->
            match movement with
            | Move.Backward xLines ->
                noMessage
            | Move.Forward xLines ->
                noMessage
        | _ ->
            noMessage

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
