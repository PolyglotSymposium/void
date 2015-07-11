namespace Void.Core.Spec

open Void.Core
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Editing command mode``() = 
    let interpret_success request = InterpretScriptFragmentResponse.Completed
    let error = Error.ScriptFragmentParseFailed "Augh!"
    let interpret_parseFailure request = InterpretScriptFragmentResponse.ParseFailed error
    let interpret_parseIncomplete request = InterpretScriptFragmentResponse.ParseIncomplete
    let enter = TextOrHotKey.HotKey HotKey.Enter
    let escape = TextOrHotKey.HotKey HotKey.Escape
    let backspace = TextOrHotKey.HotKey HotKey.Backspace

    let typeIncrement increment buffer expected =
        TextOrHotKey.Text increment
        |> CommandMode.handle interpret_success buffer
        |> should equal (expected, CommandMode.Event.TextAppended increment :> Message)

    [<Test>]
    member x.``Text can be incrementally typed in``() =
        typeIncrement "e" "" "e"
        typeIncrement "d" "e" "ed"
        typeIncrement "i" "ed" "edi"
        typeIncrement "t" "edi" "edit"

    [<Test>]
    member x.``When enter is pressed, the command text is interpreted``() =
        let commandForInterpreting = ref ""
        let fakeInterpret request =
            commandForInterpreting := request.Fragment
            InterpretScriptFragmentResponse.Completed
        CommandMode.handle fakeInterpret "edit" enter |> ignore
        !commandForInterpreting |> should equal "edit"

    [<Test>]
    member x.``When escape is pressed, command entry is cancelled``() =
        CommandMode.handle interpret_success "edit" escape
        |> should equal ("", CommandMode.Event.EntryCancelled :> Message)

    [<Test>]
    member x.``When backspace is pressed, the previous character is remove from the buffer``() =
        CommandMode.handle interpret_success "edig" backspace
        |> should equal ("edi", CommandMode.Event.CharacterBackspaced :> Message)

    [<Test>]
    member x.``When backspace is pressed and there are no characters but the prompt, command entry is cancelled``() =
        CommandMode.handle interpret_success "" backspace
        |> should equal ("", CommandMode.Event.EntryCancelled :> Message)

    [<Test>]
    member x.``When the command text is parsed successfully, the command text is reset``() =
        CommandMode.handle interpret_success "edit" enter
        |> should equal ("", CommandMode.Event.CommandCompleted  "edit" :> Message)

    [<Test>]
    member x.``When the command text is not parsed successfully, the command text is reset``() =
        CommandMode.handle interpret_parseFailure "edit" enter
        |> should equal ("", CoreEvent.ErrorOccurred error :> Message)

    [<Test>]
    member x.``When the command text parse is incomplete, a newline is added to the command text``() =
        CommandMode.handle interpret_parseIncomplete "edit" enter
        |> fst
        |> should equal ("edit" + System.Environment.NewLine)
