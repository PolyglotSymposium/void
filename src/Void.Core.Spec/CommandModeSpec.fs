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

    let typeIncrement increment buffer expected =
        TextOrHotKey.Text increment
        |> CommandMode.handle interpret_success buffer
        |> fst
        |> should equal expected

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
        |> should equal ("", Some Event.CommandEntryCancelled)

    [<Test>]
    member x.``When the command text is parsed successfully, the command text is reset``() =
        CommandMode.handle interpret_success "edit" enter
        |> should equal ("", Some Event.LineCommandCompleted)

    [<Test>]
    member x.``When the command text is not parsed successfully, the command text is reset``() =
        CommandMode.handle interpret_parseFailure "edit" enter
        |> should equal ("", Some <| Event.ErrorOccurred error)

    [<Test>]
    member x.``When the command text parse is incomplete, a newline is added to the command text``() =
        CommandMode.handle interpret_parseIncomplete "edit" enter
        |> fst
        |> should equal "edit\n"
