namespace Void.ViewModel

module RenderNotificationBar =
    open Void.Core
    open Void.Core.CellGrid

    let private notificationAsDrawingObject upperLeft notification =
        match notification with
        | UserNotificationView.Text text ->
            {
                Text = text
                UpperLeftCorner = GridConvert.upperLeftCornerOf upperLeft
                Color = Colors.defaultColorscheme.Foreground
            }
        | UserNotificationView.Error text ->
            {
                Text = text
                UpperLeftCorner = GridConvert.upperLeftCornerOf upperLeft
                Color = Colors.defaultColorscheme.Error
            }
        |> DrawingObject.Text

    let asDrawingObjects width upperLeft notifications =
        let asDrawingObject =
            notificationAsDrawingObject upperLeft
        notifications |> List.map asDrawingObject

    let private toScreenNotification =
        function
        | UserNotification.Output notificationText -> UserNotificationView.Text notificationText
        | UserNotification.Error error -> UserNotificationView.Error <| Errors.textOf error

    let handleEvent notificationsOrigin event =
        match event with
        | CoreEvent.NotificationAdded notification ->
            let area = {
                UpperLeftCell = !notificationsOrigin
                Dimensions = { Rows = 1<mRow>; Columns = 80<mColumn> }
            }
            let drawing = toScreenNotification notification
                          |> notificationAsDrawingObject !notificationsOrigin
            let areaInPoints = GridConvert.boxAround area
            VMEvent.ViewPortionRendered(areaInPoints, [drawing]) :> Message
        | _ -> noMessage

    [<RequireQualifiedAccess>]
    type Event =
        | NotificationBarOriginReset of CellGrid.Cell
        interface EventMessage

    let handleVMEvent notificationBarOrigin event =
        match event with
        | VMEvent.ViewModelInitialized viewModel ->
            let newOrigin = ViewModel.upperLeftCellOfCommandBar viewModel
            newOrigin, Event.NotificationBarOriginReset newOrigin :> Message
        | _ -> notificationBarOrigin, noMessage

    module Service =
        let subscribe (bus : Bus) =
            let notificationBarOrigin = ref originCell
            handleEvent notificationBarOrigin |> bus.subscribe
            Service.wrap notificationBarOrigin handleVMEvent |> bus.subscribe

