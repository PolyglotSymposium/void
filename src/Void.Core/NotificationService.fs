namespace Void.Core

type NotificationService() =
    let mutable _notifications = []

    member x.handleEvent event =
        match event with
        | Event.ErrorOccurred error ->
            // TODO ...Or should the underlying model create the events?
            let notifications, event = Notifications.addError error _notifications
            _notifications <- notifications
            Command.PublishEvent event
        | _ -> Command.Noop

    member x.handleCommand command =
        match command with
        | Command.ShowNotificationHistory ->
            Command.Display <| Displayable.Notifications _notifications
        | _ -> Command.Noop

