namespace Void

open Void.Core

module MessageLog =
    let handleMessage message =
        printf "Message Published: %A\n" message
        noMessage

    module Service =
        let subscribe (bus : Bus) =
            bus.subscribe handleMessage 