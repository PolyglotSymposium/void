namespace Void.ViewModel

module CommandBar =
    open Void.Core
    open Void.Core.CellGrid
    open Void.Util

    let hidden =
        {
            Width = 80
            Prompt = Hidden
            WrappedLines = []
        }

    let visibleButEmpty =
        {
            Width = 80
            Prompt = Visible CommandBarPrompt.VoidDefault
            WrappedLines = [""]
        }

    [<RequireQualifiedAccess>]
    type Event =
        | CharacterBackspacedFromLine of CellGrid.Cell
        | Displayed of CommandBarView
        | Hidden of CommandBarView
        | TextAppendedToLine of SegmentOfText
        | TextChanged of CommandBarView
        | TextReflowed of CommandBarView
        interface EventMessage

    let private lastCell commandBar = 
        CellGrid.rightOf CellGrid.originCell commandBar.WrappedLines.Head.Length

    let private currentLineWillOverflow (textToAppend : string) commandBar =
        if commandBar.WrappedLines = [] // TODO this is a hack to avoid an exception
        then false
        else
            let length = commandBar.WrappedLines.Head.Length + textToAppend.Length
            length >= commandBar.Width ||
            (commandBar.WrappedLines.Length = 1 && length + 1 = commandBar.Width)

    let private appendText textToAppend commandBar =
        if currentLineWillOverflow textToAppend commandBar
        then
            let bar = { commandBar with WrappedLines = textToAppend :: commandBar.WrappedLines }
            (bar, Event.TextReflowed bar :> Message)
        else
            // TODO: I ran into an exception here
            let line = commandBar.WrappedLines.Head + textToAppend
            let bar = { commandBar with WrappedLines = line :: commandBar.WrappedLines.Tail }
            let textSegment = { LeftMostCell = CellGrid.rightOf (lastCell commandBar) 1; Text = textToAppend }
            (bar, Event.TextAppendedToLine textSegment :> Message)

    let private characterBackspaced commandBar =
        let backspacedLine = StringUtil.backspace commandBar.WrappedLines.Head
        if backspacedLine = ""
        then
            let bar = { commandBar with WrappedLines =  commandBar.WrappedLines.Tail }
            in (bar, Event.TextReflowed bar :> Message)
        else
            let lines = backspacedLine :: commandBar.WrappedLines.Tail
            let bar = { commandBar with WrappedLines = lines }
            let clearedCell = lastCell commandBar
            (bar, Event.CharacterBackspacedFromLine clearedCell :> Message)

    let private hide =
        let commandBar = hidden
        (commandBar, Event.Hidden commandBar :> Message)

    let private show =
        let commandBar = visibleButEmpty
        in (commandBar, Event.Displayed commandBar :> Message)

    let handleEvent commandBar event =
        match event with
        | CoreEvent.ModeChanged { From = _; To = Mode.Command } -> show
        | _ -> (commandBar, noMessage)

    let handleCommandModeEvent commandBar event =
        match event with
        | CommandMode.Event.EntryCancelled -> hide
        | CommandMode.Event.CharacterBackspaced -> characterBackspaced commandBar
        | CommandMode.Event.TextAppended text -> appendText text commandBar
        | CommandMode.Event.CommandCompleted -> (commandBar, noMessage)

module CommandBarService =
    open Void.Core

    let build() =
        let commandBar = ref CommandBar.hidden
        let eventHandler = Service.wrap commandBar CommandBar.handleEvent
        let commandModeEventHandler = Service.wrap commandBar CommandBar.handleCommandModeEvent
        (eventHandler, commandModeEventHandler)