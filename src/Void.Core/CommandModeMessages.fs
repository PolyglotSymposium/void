namespace Void.Core

type InterpretFullScriptRequest =
    {
        Language : string
        Script : string
    }
    interface RequestMessage

[<RequireQualifiedAccess>]
type InterpretFullScriptResponse =
    | ParseFailed of Error
    | Completed
    interface ResponseMessage<InterpretFullScriptRequest>

type InterpretScriptFragmentRequest =
    {
        Language : string
        Fragment : string
    }
    interface RequestMessage

[<RequireQualifiedAccess>]
type InterpretScriptFragmentResponse =
    | ParseFailed of Error
    | ParseIncomplete
    | Completed
    interface ResponseMessage<InterpretScriptFragmentRequest>

[<RequireQualifiedAccess>]
type CommandHistoryCommand =
    | MoveToPreviousCommand
    | MoveToNextCommand
    interface CommandMessage

[<RequireQualifiedAccess>]
type CommandHistoryEvent =
    | MovedToCommand of string
    | MovedToEmptyCommand
    | CommandAdded
    interface Message

type GetCurrentCommandLanguageRequest =
    | GetCurrentCommandLanguageRequest
    interface RequestMessage

type GetCurrentCommandLanguageResponse =
    {
        CurrentCommandLanguage : string
    }
    interface ResponseMessage<GetCurrentCommandLanguageRequest>

