namespace Void.Core

module MessageLog =
    let handleMessage message =
        printf "Message Published: %A\n" message
        noMessage

    module Service =
        let subscribe (subscribeHandler : SubscribeToBus) =
            subscribeHandler.subscribe handleMessage 