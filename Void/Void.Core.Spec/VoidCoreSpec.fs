namespace Void.Core.Spec

open Void.Core
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``When the editor is launched without any command-line arguments``() = 
    [<Test>]
    member x.``it should open exactly one empty file buffer``() =
        let editorState = Editor.init CommandLine.noArgs
        editorState.BufferList.Length |> should equal 1
        editorState.CurrentBuffer |> should equal 0
        editorState.BufferList.[editorState.CurrentBuffer] |> should equal Buffer.empty
