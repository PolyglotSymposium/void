namespace Void.ViewModel

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
        | CursorMoved of From : Cell * To : Cell * Window : WindowView
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
        { zeroWindowView with Dimensions = (lessRowsBelow 1<mRow> containingArea).Dimensions }

    let defaultWindowView =
        windowInArea zeroWindowView Sizing.defaultViewArea

    let private linesInWindow window = 
        window.Buffer.Length*1<mLine>

    let bufferFrom (windowSize : Dimensions) lines =
        let truncateToWindowWidth = StringUtil.noLongerThan (windowSize.Columns / 1<mColumn>)
        lines
        |> SeqUtil.notMoreThan (windowSize.Rows / 1<mRow>)
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
        | BufferEvent.CursorMoved(fromCell, toCell) ->
            let updatedWindow = { window with Cursor = Visible (CursorView.Block toCell)}
            updatedWindow, Event.CursorMoved(fromCell, toCell, updatedWindow) :> Message

    let private scroll (requestSender : RequestSender) window xLines =
        let request : GetWindowContentsRequest = { StartingAtLine = window.TopLineNumber + xLines }
        match requestSender.makeRequest request with
        | Some (response : GetWindowContentsResponse) ->
            let updatedWindow = { window with TopLineNumber = response.FirstLineNumber; Buffer = Seq.toList response.RequestedContents }
            updatedWindow, Event.ContentsUpdated updatedWindow :> Message
        | None -> window, noMessage

    let scrollByLineMovement requestSender window movement =
        let noScroll = window, noMessage
        match movement with
        | Move.Backward (By.Line xLines) ->
            let scrollAmount =
                if window.TopLineNumber > xLines
                then xLines
                else window.TopLineNumber - 1<mLine>
            if scrollAmount > 0<mLine>
            then scroll requestSender window -scrollAmount
            else noScroll
        | Move.Forward (By.Line xLines) ->
            let scrollAmount =
                if linesInWindow window > xLines
                then xLines
                else linesInWindow window - 1<mLine>
            if scrollAmount > 0<mLine>
            then scroll requestSender window scrollAmount
            else noScroll

    let scrollHalfScreenHeights requestSender (window : WindowView) movement =
        let toLines (screenHeights : int<mScreenHeight>) =
            window.Dimensions.Rows / 2<mScreenHeight> * screenHeights * linePerRow
            |> By.Line
        match movement with
        | Move.Backward (vmBy.ScreenHeight screenHeights) ->
            toLines screenHeights |> Move.Backward
        | Move.Forward (vmBy.ScreenHeight screenHeights) ->
            toLines screenHeights |> Move.Forward
        |> scrollByLineMovement requestSender window

    let handleVMCommand requestSender window command =
        let noScroll = window, noMessage
        match command with
        | VMCommand.Scroll movement ->
            scrollByLineMovement requestSender window movement
        | VMCommand.ScrollHalf movement ->
            scrollHalfScreenHeights requestSender window movement
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
