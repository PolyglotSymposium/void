namespace Void.Core


type InterpretFullScriptRequest = {
    Language : string
    Script : string
}

[<RequireQualifiedAccess>]
type InterpretFullScriptResponse =
    | ParseFailed of Error
    | Completed

type InterpretScriptFragmentRequest = {
    Language : string
    Fragment : string
}

[<RequireQualifiedAccess>]
type InterpretScriptFragmentResponse =
    | ParseFailed of Error
    | ParseIncomplete
    | Completed

module RequestAPI =
    type InterpretScriptFragment = InterpretScriptFragmentRequest -> InterpretScriptFragmentResponse
    type InterpretFullScript = InterpretFullScriptRequest -> InterpretFullScriptResponse