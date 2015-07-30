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
        let subscribe (subscribeHandler : SubscribeToBus) =
            let language = ref voidScript
            subscribeHandler.subscribeToRequest (handleRequest language)
            subscribeHandler.subscribe <| Service.wrap language handleCommand
            subscribeHandler.subscribe <| Service.wrap language handleNoResponseToInterpretFragmentRequest