namespace Void.ViewModel

[<RequireQualifiedAccess>]
type UserNotificationView =
    | Text of string
    | Error of string
