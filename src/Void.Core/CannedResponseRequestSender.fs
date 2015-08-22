namespace Void.Core

(* This is really for tests, and probably shouldn't be here.
 * However, I'm not going to extract a new assembly for this one class.
 * Besides, the test DLL shouldn't depend on Void.Core.
 * Void.Core has become unclear anyway: it really should be Void.Base and Void.Editor,
 * or something to that effect, and the test DLL should only depend on Void.Base.
 * So TODO: when there are several more things like this, create a test library.
 * Until then, this code isn't really hurting anybody. *)
type CannedResponseRequestSender() =
    let mutable _requests = []
    let mutable _responses = []

    member x.registerResponse<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> (response : 'TResponse) =
        _responses <- box response :: _responses

    member x.tryPickRequest<'TRequest when 'TRequest :> RequestMessage>() =
        let tryUnbox request =
            try
                unbox<'TRequest> request |> Some
            with _ -> 
                None
        _requests |> List.tryPick tryUnbox

    member x.reset() =
        _requests <- []
        _responses <- []

    interface RequestSender with
        member x.makeRequest<'TRequest, 'TResponse when 'TRequest :> RequestMessage and 'TResponse :> ResponseMessage<'TRequest>> (request : 'TRequest) =
            let tryUnbox response =
                try
                    unbox<'TResponse> response |> Some
                with _ -> 
                    None
            _requests <- box request :: _requests
            _responses |> List.tryPick tryUnbox

