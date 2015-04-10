namespace Void.Core.Spec

open Void.Core
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Editing command mode``() = 
    let noopInterpret request = InterpretScriptFragmentResponse.Completed

    let typeIncrement increment buffer expected =
        TextOrHotKey.Text increment
        |> CommandMode.handle noopInterpret buffer
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
        CommandMode.handle fakeInterpret "edit" (TextOrHotKey.HotKey HotKey.Enter) |> ignore
        !commandForInterpreting |> should equal "edit"

    [<Test>]
    member x.``When the command text is parsed successfully, the command text is reset``() =
        false |> should be True

    [<Test>]
    member x.``When the command text is not parsed successfully, the command text is reset``() =
        false |> should be True

    [<Test>]
    member x.``When the command text parse is incomplete, a newline is added to the command text``() =
        false |> should be True

    [<Test>]
    member x.``When the command text is parsed successfully, the mode is changed to normal``() =
        false |> should be True

    [<Test>]
    member x.``When the command text is not parsed successfully, an error is published``() =
        // This will require publishing multiple commands, which will be a design change
        // that I have been wondering about the need for for a while
        false |> should be True

    [<Test>]
    member x.``When the command text is not parsed successfully, the mode is changed to normal``() =
        false |> should be True

    [<Test>]
    member x.``When the command text parse is incomplete, the mode is not changed``() =
        false |> should be True
