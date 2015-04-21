namespace Void.Core

type NotificationService() =
    let mutable _notifications = []

    member x.handleEvent =
        function
        | Event.ErrorOccurred error ->
            // TODO ...Or should the underlying model create the events?
            let notifications, event = Notifications.addError error _notifications
            _notifications <- notifications
            event :> Message
        | _ -> noMessage

    member x.handleCommand =
        function
        | Command.ShowNotificationHistory ->
            Command.Display <| Displayable.Notifications _notifications :> Message
        | _ -> noMessage

