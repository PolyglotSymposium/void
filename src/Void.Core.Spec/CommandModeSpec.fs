namespace Void.Core.Spec

open Void.Core
open NUnit.Framework
open FsUnit

type RequestSenderStub() =
    interface RequestSender with
        member x.makeRequest _ =
            None

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

    [<Test>]
    member x.``Text can be incrementally typed in``() =
        typeIncrement "e" "" "e"
        typeIncrement "d" "e" "ed"
        typeIncrement "i" "ed" "edi"
        typeIncrement "t" "edi" "edit"

    [<Test>]
    member x.``When enter is pressed, the current language for command mode is requested``() =
        CommandMode.handle requestSenderStub "edit" enter
        |> should equal ("edit", GetCurrentCommandLanguageRequest :> Message)

    [<Test>]
    member x.``When the current language is received, the command is interpreted for that language``() =
        let command = ref "edit"
        CommandMode.handleGetCurrentCommandLanguageResponse command { CurrentCommandLanguage = "python3" }
        |> should equal ({ Language = "python3"; Fragment = "edit" } :> Message)

    [<Test>]
    member x.``When there is no response to the request for the current language, the command is interpreted as VoidScript``() =
        let command = ref "edit"
        CommandMode.handleNoResponseToGetCurrentCommandLanguage command { Request = GetCurrentCommandLanguageRequest }
        |> should equal ({ Language = "VoidScript"; Fragment = "edit" } :> Message)

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

    [<Test>]
    member x.``When the command text is parsed successfully, the command text is reset``() =
        CommandMode.handleInterpretFragmentResponse "edit" success
        |> should equal ("", CommandMode.Event.CommandCompleted  "edit" :> Message)

    [<Test>]
    member x.``When the command text is not parsed successfully, the command text is reset``() =
        CommandMode.handleInterpretFragmentResponse "edit" parseFailure
        |> should equal ("", CoreEvent.ErrorOccurred error :> Message)

    [<Test>]
    member x.``When the command text parse is incomplete, a newline is added to the command text``() =
        CommandMode.handleInterpretFragmentResponse "edit" parseIncomplete 
        |> fst
        |> should equal ("edit" + System.Environment.NewLine)
