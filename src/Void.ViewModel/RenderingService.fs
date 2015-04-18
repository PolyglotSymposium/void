namespace Void.ViewModel

open Void.Core

type RenderingService
    (
        _view : MainView,
        _rerenderView : RerenderWholeView
    ) =
    let mutable _bufferedDrawings = Seq.empty

    member private x.bufferDrawings drawings =
        _bufferedDrawings <- Seq.append _bufferedDrawings drawings

    member private x.paint draw =
        let drawAll drawings = for drawing in drawings do draw drawing
        if Seq.isEmpty _bufferedDrawings
        then drawAll <| _rerenderView()
        else drawAll _bufferedDrawings
        _bufferedDrawings <- []

    member x.handleVMCommand command =
        match command with
        | VMCommand.Redraw area ->
            _view.TriggerDraw area
        noMessage

    member x.handleVMEvent event =
        match event with
        | VMEvent.PaintInitiated draw ->
            x.paint draw
        | VMEvent.ViewPortionRendered (area, drawings) ->
            x.bufferDrawings drawings
            _view.TriggerDraw area
        | _ -> ()
        noMessage
