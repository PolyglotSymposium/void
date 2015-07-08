﻿namespace Void.ViewModel

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

    let private lastCell commandBar = 
        CellGrid.rightOf CellGrid.originCell commandBar.WrappedLines.Head.Length

    let private currentLineWillOverflow (textToAppend : string) commandBar =
        // TODO I get an exception here sometimes
        let length = commandBar.WrappedLines.Head.Length + textToAppend.Length
        length >= commandBar.Width ||
        (commandBar.WrappedLines.Length = 1 && length + 1 = commandBar.Width)

    let private appendText textToAppend commandBar =
        if currentLineWillOverflow textToAppend commandBar
        then
            let bar = { commandBar with WrappedLines = textToAppend :: commandBar.WrappedLines }
            (bar, VMEvent.CommandBar_TextReflowed bar :> Message)
        else
            let line = commandBar.WrappedLines.Head + textToAppend
            let bar = { commandBar with WrappedLines = line :: commandBar.WrappedLines.Tail }
            let textSegment = { LeftMostCell = CellGrid.rightOf (lastCell commandBar) 1; Text = textToAppend }
            (bar, VMEvent.CommandBar_TextAppendedToLine textSegment :> Message)

    let private characterBackspaced commandBar =
        let backspacedLine = StringUtil.backspace commandBar.WrappedLines.Head
        if backspacedLine = ""
        then
            let bar = { commandBar with WrappedLines =  commandBar.WrappedLines.Tail }
            in (bar, VMEvent.CommandBar_TextReflowed bar :> Message)
        else
            let lines = backspacedLine :: commandBar.WrappedLines.Tail
            let bar = { commandBar with WrappedLines = lines }
            let clearedCell = lastCell commandBar
            (bar, VMEvent.CommandBar_CharacterBackspacedFromLine clearedCell :> Message)

    let private hide =
        let commandBar = hidden
        (commandBar, VMEvent.CommandBar_Hidden commandBar :> Message)

    let private show =
        let commandBar = visibleButEmpty
        in (commandBar, VMEvent.CommandBar_Displayed commandBar :> Message)

    let handleEvent commandBar event =
        match event with
        | Event.ModeChanged { From = _; To = Mode.Command } -> show
        | Event.CommandEntryCancelled -> hide
        | Event.CommandMode_CharacterBackspaced -> characterBackspaced commandBar
        | Event.CommandMode_TextAppended text -> appendText text commandBar
        | _ -> (commandBar, noMessage)

module CommandBarService =
    open Void.Core

    let private eventHandler commandBar =
        Service.wrap commandBar CommandBar.handleEvent

    let build() =
        let commandBar = ref CommandBar.hidden
        eventHandler commandBar