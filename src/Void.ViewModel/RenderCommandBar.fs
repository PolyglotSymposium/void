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

    let private backspacedCharacterAsDrawingObject cell origin =
        let offsetCell = CellGrid.vectorAdd origin cell 
        let area = GridConvert.boxAroundOneCell offsetCell
        let drawing = DrawingObject.Block {
            Area = area
            Color = Colors.defaultColorscheme.Background
        }
        area, Seq.singleton drawing

    let private appendedTextAsDrawingObject textSegment origin =
        let offsetCell = CellGrid.vectorAdd origin textSegment.LeftMostCell 
        let area = GridConvert.boxAroundOneCell offsetCell // TODO but what if it's not one cell?
        let drawing = DrawingObject.Text {
            Text = textSegment.Text
            UpperLeftCorner = GridConvert.upperLeftCornerOf offsetCell
            Color = Colors.defaultColorscheme.Foreground
        }
        area, Seq.singleton drawing

    let asDrawingObjects commandBar origin =
        let height =
            if commandBar = CommandBar.hidden
            then 1
            else commandBar.WrappedLines.Length

        let area : PointGrid.Block = {
            UpperLeftCorner = GridConvert.upperLeftCornerOf origin
            Dimensions = { Height = height; Width = commandBar.Width }
        }

        area, render commandBar area origin

    let private renderCommandBar commandBar commandBarOrigin =
        asDrawingObjects commandBar !commandBarOrigin
        |> VMEvent.ViewPortionRendered :> Message

    let handleCommandBarEvent commandBarOrigin event =
        match event with
        | CommandBar.Event.CharacterBackspacedFromLine cell ->
            backspacedCharacterAsDrawingObject cell !commandBarOrigin
            |> VMEvent.ViewPortionRendered :> Message
        | CommandBar.Event.Displayed commandBar ->
            renderCommandBar commandBar commandBarOrigin
        | CommandBar.Event.Hidden commandBar ->
            renderCommandBar commandBar commandBarOrigin
        | CommandBar.Event.TextAppendedToLine textSegment ->
            appendedTextAsDrawingObject textSegment !commandBarOrigin
            |> VMEvent.ViewPortionRendered :> Message
        | CommandBar.Event.TextChanged commandBar ->
            renderCommandBar commandBar commandBarOrigin
        | CommandBar.Event.TextReflowed commandBar ->
            renderCommandBar commandBar commandBarOrigin

    let handleCommandBarCommand commandBarOrigin (CommandBar.Command.Redraw commandBar) =
        renderCommandBar commandBar commandBarOrigin

    [<RequireQualifiedAccess>]
    type Event =
        | CommandBarOriginReset of CellGrid.Cell
        interface EventMessage

    let handleVMEvent commandBarOrigin event =
        match event with
        | VMEvent.ViewModelInitialized viewModel ->
            let newOrigin = ViewModel.upperLeftCellOfCommandBar viewModel
            newOrigin, Event.CommandBarOriginReset newOrigin :> Message
        | _ -> commandBarOrigin, noMessage

    module Service =
        let subscribe (bus : Bus) =
            let commandBarOrigin = ref CellGrid.originCell
            handleCommandBarEvent commandBarOrigin |> bus.subscribe
            handleCommandBarCommand commandBarOrigin |> bus.subscribe
            Service.wrap commandBarOrigin handleVMEvent |> bus.subscribe
