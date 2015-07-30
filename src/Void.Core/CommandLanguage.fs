namespace Void.Core

module CommandLanguage =

    type Command =
        | ChangeCurrentCommandLanguageTo of string
        interface CommandMessage

    type Event =
        | CurrentCommandLanguageChangedTo of string
        interface CommandMessage

    let voidScript = "VoidScript"

    let handleRequest language request =
        {
            CurrentCommandLanguage = !language
        }

    let handleCommand _ (ChangeCurrentCommandLanguageTo newLanguage) =
        newLanguage, CurrentCommandLanguageChangedTo newLanguage :> Message

    let handleNoResponseToInterpretFragmentRequest _ (msg : NoResponseToRequest<InterpretScriptFragmentRequest>) =
        voidScript, CurrentCommandLanguageChangedTo voidScript :> Message

    module Service =
        let subscribe (bus : Bus) =
            let language = ref voidScript
            bus.subscribeToRequest (handleRequest language)
            bus.subscribe <| Service.wrap language handleCommand
            bus.subscribe <| Service.wrap language handleNoResponseToInterpretFragmentRequest