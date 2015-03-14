namespace Void.Lang.Parser.Spec

open Void.Lang.Parser
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Parser``() = 
    [<Test>]
    member x.``should be bootstrapped``() =
        Parser.bootstrapped |> should be True
