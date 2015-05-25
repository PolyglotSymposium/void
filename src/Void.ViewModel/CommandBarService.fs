namespace Void.ViewModel

module CommandBarV2 =
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

    let appendText (commandBar : CommandBarView) area textToAppend =
        let bar = { commandBar with Text = commandBar.Text + textToAppend }
        let areaInPoints = GridConvert.boxAround {
            UpperLeftCell = { area.UpperLeftCell with Column = area.UpperLeftCell.Column + 1 + commandBar.Text.Length }
            Dimensions = { Columns = textToAppend.Length; Rows = 1 }
        }
        let drawing = DrawingObject.Text {
            UpperLeftCorner = areaInPoints.UpperLeftCorner
            Text = textToAppend
            Color = Colors.defaultColorscheme.Foreground
        }
        (bar, VMEvent.ViewPortionRendered(areaInPoints, [drawing]) :> Message)

    let characterBackspaced (commandBar : CommandBarViewV2) =
        let backspacedLine = StringUtil.backspace commandBar.WrappedLines.Head
        if backspacedLine = ""
        then
            let bar = { commandBar with WrappedLines = List.tail commandBar.WrappedLines }
            in (bar, VMEvent.CommandBar_TextReflowed bar :> Message)
        else
            let lines = backspacedLine :: List.tail commandBar.WrappedLines
            let bar = { commandBar with WrappedLines = lines }
            let clearedCell = CellGrid.rightOf CellGrid.originCell commandBar.WrappedLines.Head.Length
            (bar, VMEvent.CommandBar_CharacterBackspacedFromLine clearedCell :> Message)

    let hide =
        (hidden, VMEvent.CommandBar_Hidden :> Message)

    let show =
        let commandBar = visibleButEmpty
        in (commandBar, VMEvent.CommandBar_Displayed commandBar :> Message)

    let handleEvent commandBar event =
        match event with
        | Event.ModeChanged { From = _; To = Mode.Command } -> show
        | Event.CommandEntryCancelled -> hide
        | Event.CommandMode_CharacterBackspaced -> characterBackspaced commandBar
        | Event.CommandMode_TextAppended text ->
            (hidden, noMessage)
        | _ -> (commandBar, noMessage)

module CommandBarService =
    open Void.Core

    let eventHandler commandBar =
        Service.wrap commandBar CommandBarV2.handleEvent