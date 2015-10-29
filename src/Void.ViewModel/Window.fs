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
    Cursor : CursorView
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
            Cursor = { Position = originCell; Appearance = Visible CursorStyle.Block }
            TopLineNumber = 1<mLine>
        }

    let private windowInArea window containingArea =
        { zeroWindowView with Dimensions = (lessRowsBelow 1<mRow> containingArea).Dimensions }

    let defaultWindowView =
        windowInArea zeroWindowView Sizing.defaultViewArea

    let private linesInWindow window = 
        window.Buffer.Length*1<mLine>

    let private lastPopulatedRow window = 
        ``line#->row#`` <| linesInWindow window

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

    let setCursorPosition window cell =
        { window with Cursor = { window.Cursor with Position = cell } }

    let private firstRowOf window =
        ``line#->row#`` window.TopLineNumber

    let private lastRowOf (window : WindowView) =
        window.Dimensions.Rows - 1<mRow>

    let private scrollUpSinceCursorMovedAboveTop window toBufferCell =
        let updatedWindow = setCursorPosition window originCell
        let msg = (firstRowOf window - toBufferCell.Row) * linePerRow
                  |> By.Line
                  |> Move.Backward
                  |> VMCommand.Scroll :> Message
        updatedWindow, msg

    let private scrollUpSinceCursorMovedBelowBottom window toWindowCell =
        let updatedWindow = setCursorPosition window { originCell with Row = lastRowOf window }
        let msg = (toWindowCell.Row - lastRowOf window) * linePerRow
                  |> By.Line
                  |> Move.Forward
                  |> VMCommand.Scroll :> Message
        updatedWindow, msg

    let handleBufferEvent window event =
        match event.Message with
        | BufferEvent.Added buffer ->
            loadBufferIntoWindow buffer window
        | BufferEvent.CursorMoved(fromBufferCell, toBufferCell) ->
            let firstRow = ``line#->row#`` window.TopLineNumber
            if toBufferCell.Row < firstRow
            then scrollUpSinceCursorMovedAboveTop window toBufferCell
            else
                let toWindowCell = CellGrid.above toBufferCell firstRow
                if toWindowCell.Row > window.Dimensions.Rows - 1<mRow>
                then scrollUpSinceCursorMovedBelowBottom window toWindowCell
                else
                    let fromWindowCell = CellGrid.above fromBufferCell firstRow
                    let updatedWindow = setCursorPosition window toWindowCell
                    updatedWindow, Event.CursorMoved(fromWindowCell, toWindowCell, updatedWindow) :> Message

    let private resetCursorIfNecessary window =
        if lastPopulatedRow window < window.Cursor.Position.Row
        then
            { window.Cursor.Position with Row = lastPopulatedRow window }
            |> setCursorPosition window
        else window

    let private scroll (requestSender : RequestSender) window xLines =
        let request : GetWindowContentsRequest = { StartingAtLine = window.TopLineNumber + xLines }
        match requestSender.makeRequest request with
        | Some (response : GetWindowContentsResponse) ->
            let updatedWindow = { window with TopLineNumber = response.FirstLineNumber; Buffer = Seq.toList response.RequestedContents }
            let updatedWindow2 = resetCursorIfNecessary updatedWindow
            updatedWindow2, Event.ContentsUpdated updatedWindow2 :> Message
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
