namespace Void.Core

module Notifications =
    let private addNotification notification notifications = 
        (notification :: notifications), CoreEvent.NotificationAdded notification :> Message

    let private addError error notifications =
        let notification = UserNotification.Error error
        in addNotification notification notifications

    let private addOutput text notifications =
        let notification = UserNotification.Output text
        in addNotification notification notifications

    let private showHistory notifications =
        let msg = CoreCommand.Display <| Displayable.Notifications notifications
        in notifications, msg :> Message

    let handleEvent notifications event =
        match event with
        | CoreEvent.ErrorOccurred error ->
            addError error notifications
        | _ -> (notifications, noMessage)

    let handleCommand notifications command =
        match command with
        | CoreCommand.AddNotification notification ->
            addNotification notification notifications
        | CoreCommand.ShowNotificationHistory ->
            showHistory notifications
        | _ -> (notifications, noMessage)

    module Service =
        let subscribe (subscribeHandler : SubscribeToBus) =
            let notifications = ref []
            subscribeHandler.subscribe <| Service.wrap notifications handleEvent
            subscribeHandler.subscribe  <| Service.wrap notifications handleCommand
             