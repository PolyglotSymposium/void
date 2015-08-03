namespace Void.Core.Spec

open Void.Core
open NUnit.Framework
open FsUnit

type RequestSenderStub() =
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

[<TestFixture>]
type ``Editing command mode``() = 
    let success = InterpretScriptFragmentResponse.Completed
    let error = Error.ScriptFragmentParseFailed ("Augh!", "Psst!")
    let parseFailure = InterpretScriptFragmentResponse.ParseFailed error
    let parseIncomplete = InterpretScriptFragmentResponse.ParseIncomplete
    let enter = TextOrHotKey.HotKey HotKey.Enter
    let escape = TextOrHotKey.HotKey HotKey.Escape
    let backspace = TextOrHotKey.HotKey HotKey.Backspace
    let requestSenderStub = RequestSenderStub()

    let typeIncrement increment buffer expected =
        TextOrHotKey.Text increment
        |> CommandMode.handle requestSenderStub buffer
        |> should equal (expected, CommandMode.Event.TextAppended increment :> Message)

    [<SetUp>]
    member x.``Set up``() =
        requestSenderStub.reset()

    [<Test>]
    member x.``Text can be incrementally typed in``() =
        typeIncrement "e" "" "e"
        typeIncrement "d" "e" "ed"
        typeIncrement "i" "ed" "edi"
        typeIncrement "t" "edi" "edit"

    [<Test>]
    member x.``When enter is pressed, and the current language response comes back, then the fragment interpretation request is sent for that language``() =
        requestSenderStub.registerResponse { CurrentCommandLanguage = "python3" }
        CommandMode.handle requestSenderStub "edit" enter |> ignore
        requestSenderStub.tryPickRequest<InterpretScriptFragmentRequest>()
        |> should equal (Some { Language = "python3"; Fragment = "edit"})

    [<Test>]
    member x.``When enter is pressed, and no response comes back for the current language, then the fragment interpretation request is sent with the default language``() =
        CommandMode.handle requestSenderStub "edit" enter |> ignore
        requestSenderStub.tryPickRequest<InterpretScriptFragmentRequest>()
        |> should equal (Some { Language = "VoidScript"; Fragment = "edit"})

    [<Test>]
    member x.``When the command text is parsed successfully, the command text is reset``() =
        requestSenderStub.registerResponse success
        CommandMode.handle requestSenderStub "edit" enter
        |> should equal ("", CommandMode.Event.CommandCompleted "edit" :> Message)

    [<Test>]
    member x.``When the command text is not parsed successfully, the command text is reset``() =
        requestSenderStub.registerResponse parseFailure
        CommandMode.handle requestSenderStub "edit" enter
        |> should equal ("", CoreEvent.ErrorOccurred error :> Message)

    [<Test>]
    member x.``When the command text parse is incomplete, a newline is added to the command text``() =
        requestSenderStub.registerResponse parseIncomplete
        CommandMode.handle requestSenderStub "edit" enter
        |> should equal ("edit" + System.Environment.NewLine, CommandMode.NewlineAppended :> Message)

    [<Test>]
    member x.``When escape is pressed, command entry is cancelled``() =
        CommandMode.handle requestSenderStub "edit" escape
        |> should equal ("", CommandMode.Event.EntryCancelled :> Message)

    [<Test>]
    member x.``When backspace is pressed, the previous character is remove from the buffer``() =
        CommandMode.handle requestSenderStub "edig" backspace
        |> should equal ("edi", CommandMode.Event.CharacterBackspaced :> Message)

    [<Test>]
    member x.``When backspace is pressed and there are no characters but the prompt, command entry is cancelled``() =
        CommandMode.handle requestSenderStub "" backspace
        |> should equal ("", CommandMode.Event.EntryCancelled :> Message)
