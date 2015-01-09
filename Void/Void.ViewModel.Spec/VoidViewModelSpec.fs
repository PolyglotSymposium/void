namespace Void.ViewModel.Spec

open Void.ViewModel
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Void view model``() = 
    [<Test>]
    member x.``should be bootstrapped properly``() =
        Scope.bootstrapped |> should be True
