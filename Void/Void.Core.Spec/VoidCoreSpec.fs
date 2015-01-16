namespace Void.Core.Spec

open Void.Core
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``When the editor is launched without any command-line arguments``() = 
    [<Test>]
    member x.``it should open an empty buffer``() =
        let editorState = Editor.init CommandLine.noArgs
        editorState.CurrentBuffer.Buffer.Filepath |> should equal None
        editorState.CurrentBuffer.Buffer.Contents |> should equal ""

    [<Test>]
    member x.``it should have only one buffer in the buffer list``() =
        let editorState = Editor.init CommandLine.noArgs
        editorState.BufferList.Length |> should equal 1
        editorState.BufferList.[0].Filepath |> should equal None
        editorState.BufferList.[0].Contents |> should equal ""

    [<Test>]
    member x.``it should start in normal mode``() =
        let editorState = Editor.init CommandLine.noArgs
        editorState.Mode |> should equal Mode.Normal
