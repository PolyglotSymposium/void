namespace Void.ViewModel

module RenderCommandBar = 
    open Void.Core

    let private renderPrompt upperLeftCell prompt = 
        DrawingObject.Text {
            Text = match prompt with
                   | CommandBarPrompt.ClassicVim -> ":"
                   | CommandBarPrompt.VoidDefault -> ";"
            UpperLeftCorner = GridConvert.upperLeftCornerOf upperLeftCell
            Color = Colors.defaultColorscheme.DimForeground
        }

    let private renderLines upperLeftCell lines =
        let startingCellForLineNumber i =
           if i = 0
           then CellGrid.rightOf upperLeftCell 1
           else CellGrid.below upperLeftCell i
           |> GridConvert.upperLeftCornerOf

        let renderLine i text =
            DrawingObject.Text {
               Text = text
               UpperLeftCorner = startingCellForLineNumber i
               Color = Colors.defaultColorscheme.Foreground
            }

        lines
        |> List.filter (fun line -> line <> "")
        |> List.mapi renderLine

    let private render commandBar area upperLeftCell =
        DrawingObject.Block {
            Area = area
            Color = Colors.defaultColorscheme.Background
        } :: match commandBar with
             | { Prompt = Hidden; WrappedLines = _ } -> []
             | { Prompt = Visible prompt; WrappedLines = lines } ->
                 renderPrompt upperLeftCell prompt :: renderLines upperLeftCell lines
        |> Seq.ofList

    let asDrawingObjects commandBar upperLeftCell =
        let height =
            if commandBar = CommandBar.hidden
            then 1
            else commandBar.WrappedLines.Length

        let area : PointGrid.Block = {
            UpperLeftCorner = GridConvert.upperLeftCornerOf upperLeftCell
            Dimensions = { Height = height; Width = commandBar.Width }
        }

        (area, render commandBar area upperLeftCell)


