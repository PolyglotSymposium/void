namespace Void.ViewModel

module RenderCommandBar = 
    open Void.Core

    let private renderPrompt origin prompt = 
        DrawingObject.Text {
            Text = match prompt with
                   | CommandBarPrompt.ClassicVim -> ":"
                   | CommandBarPrompt.VoidDefault -> ";"
            UpperLeftCorner = GridConvert.upperLeftCornerOf origin
            Color = Colors.defaultColorscheme.DimForeground
        }

    let private renderLines origin lines =
        let startingCellForLineNumber i =
           if i = 0
           then CellGrid.rightOf origin 1
           else CellGrid.below origin i
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

    let private render commandBar area origin =
        DrawingObject.Block {
            Area = area
            Color = Colors.defaultColorscheme.Background
        } :: match commandBar with
             | { Prompt = Hidden; WrappedLines = _ } -> []
             | { Prompt = Visible prompt; WrappedLines = lines } ->
                 renderPrompt origin prompt :: renderLines origin lines
        |> Seq.ofList

    let backspacedCharacterAsDrawingObject cell origin =
        let offsetCell = CellGrid.vectorAdd origin cell 
        let area = GridConvert.boxAroundOneCell offsetCell
        let drawing = DrawingObject.Block {
            Area = area
            Color = Colors.defaultColorscheme.Background
        }
        (area, Seq.singleton drawing)

    let appendedTextAsDrawingObject textSegment origin =
        let offsetCell = CellGrid.vectorAdd origin textSegment.LeftMostCell 
        let area = GridConvert.boxAroundOneCell offsetCell // TODO but what if it's not one cell?
        let drawing = DrawingObject.Text {
            Text = textSegment.Text
            UpperLeftCorner = GridConvert.upperLeftCornerOf offsetCell
            Color = Colors.defaultColorscheme.Foreground
        }
        (area, Seq.singleton drawing)

    let asDrawingObjects commandBar origin =
        let height =
            if commandBar = CommandBar.hidden
            then 1
            else commandBar.WrappedLines.Length

        let area : PointGrid.Block = {
            UpperLeftCorner = GridConvert.upperLeftCornerOf origin
            Dimensions = { Height = height; Width = commandBar.Width }
        }

        (area, render commandBar area origin)


