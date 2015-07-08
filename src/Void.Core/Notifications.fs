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

    let handleEvent notifications event =
        match event with
        | Event.ErrorOccurred error ->
            addError error notifications
        | _ -> (notifications, noMessage)

    let handleCommand notifications command =
        match command with
        | Command.ShowNotificationHistory ->
            Command.Display <| Displayable.Notifications notifications :> Message
        | _ -> noMessage

    module Service =
        let private commandHandler notifications command =
            handleCommand !notifications command

        let build() =
            let notifications = ref []
            let eventHandler = Service.wrap notifications handleEvent
            (eventHandler, commandHandler notifications)