namespace Void.Core.Spec

open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Void core``() = 
    [<Test>]
     member x.``should be bootstrapped properly``() =
        Void.Core.Editor.bootstrapped |> should be True
