namespace Void.Model.Spec

open Void.Model
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Void core model``() = 
    [<Test>]
    member x.``should be bootstrapped properly``() =
        Editor.bootstrapped |> should be True
