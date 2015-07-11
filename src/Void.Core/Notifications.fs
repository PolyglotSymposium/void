namespace Void.Core

module Notifications =
    let private addNotification notification notifications = 
        (notification :: notifications), Event.NotificationAdded notification :> Message

    let private addError error notifications =
        let notification = UserNotification.Error error
        in addNotification notification notifications

    let private addOutput text notifications =
        let notification = UserNotification.Output text
        in addNotification notification notifications

    let private showHistory notifications =
        let msg = Command.Display <| Displayable.Notifications notifications
        in notifications, msg :> Message

    let handleEvent notifications event =
        match event with
        | Event.ErrorOccurred error ->
            addError error notifications
        | _ -> (notifications, noMessage)

    let handleCommand notifications command =
        match command with
        | Command.AddNotification notification ->
            addNotification notification notifications
        | Command.ShowNotificationHistory ->
            showHistory notifications
        | _ -> (notifications, noMessage)

    module Service =
        let build() =
            let notifications = ref []
            let eventHandler = Service.wrap notifications handleEvent
            let commandHandler = Service.wrap notifications handleCommand
            (eventHandler, commandHandler)