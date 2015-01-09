namespace Void.Model.Spec

open Void.Model
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Void core``() = 
    [<Test>]
    member x.``should be bootstrapped properly``() =
        Editor.bootstrapped |> should be True

    [<Test>]
    member x.``When I start the editor with no parameters, I get an empty buffer``() =
        Editor.bootstrapped |> should be True
