namespace Void.Core

module Notifications =
    let private addNotification notification notifications = 
        (notification :: notifications), Event.NotificationAdded notification
    let addError error notifications =
        let notification = UserNotification.Error error
        in addNotification notification notifications
    let addOutput text notifications =
        let notification = UserNotification.Output text
        in addNotification notification notifications
