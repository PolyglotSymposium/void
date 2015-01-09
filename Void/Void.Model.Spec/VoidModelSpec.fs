namespace Void.Model.Spec

open Void.Model
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Void core model``() = 
    [<Test>]
    member x.``should be bootstrapped properly``() =
        Editor.bootstrapped |> should be True

[<TestFixture>]
type ``When the editor is launched without any command-line arguments``() = 
    [<Test>]
    member x.``it should open an empty buffer``() =
        let editorState = Editor.init CommandLine.noArgs
        editorState.CurrentBuffer.Buffer.Filepath |> should equal None
        editorState.CurrentBuffer.Buffer.Contents |> should equal ""

    [<Test>]
    member x.``it should start in normal mode``() =
        let editorState = Editor.init CommandLine.noArgs
        editorState.Mode |> should equal Mode.Normal
