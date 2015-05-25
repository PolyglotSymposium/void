namespace Void.Core

module Service =
    let wrap data handle inputMsg =
        let update, outputMsg = handle !data inputMsg
        data := update
        outputMsg