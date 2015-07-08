namespace Void.Core

module Service =
    let wrap data handle inputMsg =
        let update, outputMsg = handle !data inputMsg
        if outputMsg <> noMessage
        then data := update
        outputMsg