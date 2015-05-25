namespace Void.Core

module Service =
    let wrap data handle inputMsg =
        let update, outputMsg = handle inputMsg
        data := update
        outputMsg