namespace Void.ViewModel

module CommandBarV2 =
    open Void.Core
    open Void.Core.CellGrid
    open Void.Util

    let hidden =
        { Prompt = Hidden; Text = "" }

    let visibleButEmpty =
        { Prompt = Visible CommandBarPrompt.VoidDefault; Text = "" }

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

    let characterBackspaced area (commandBar : CommandBarView) =
        let bar = {
            commandBar with Text = StringUtil.backspace commandBar.Text
        }
        let areaInPoints = GridConvert.boxAroundOneCell <| CellGrid.rightOf area.UpperLeftCell commandBar.Text.Length
        let drawing = DrawingObject.Block {
            Area = areaInPoints
            Color = Colors.defaultColorscheme.Background
        }
        (bar, VMEvent.ViewPortionRendered(areaInPoints, [drawing]) :> Message)

    let hide area =
        let commandBar = hidden
        let drawings = Render.commandBarAsDrawingObjects commandBar area.Dimensions.Columns area.UpperLeftCell
        let areaInPoints = GridConvert.boxAround area
        (commandBar, VMEvent.ViewPortionRendered(areaInPoints, drawings) :> Message)

    let show =
        let commandBar = visibleButEmpty
        in (commandBar, VMEvent.CommandBar_Displayed commandBar :> Message)

    let handleEvent =
        function
        | Event.ModeChanged { From = _; To = Mode.Command } ->
            show
        | Event.CommandEntryCancelled ->
            (CommandBar.hidden, noMessage)
        | Event.CommandMode_CharacterBackspaced ->
            (CommandBar.hidden, noMessage)
        | Event.CommandMode_TextAppended text ->
            (CommandBar.hidden, noMessage)
        | _ ->
            (CommandBar.hidden, noMessage)

module CommandBarService =
    open Void.Core

    let eventHandler data =
        Service.wrap data CommandBarV2.handleEvent