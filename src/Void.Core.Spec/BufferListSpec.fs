namespace Void.Core.BufferListSpec


open Void.Core
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``writing a buffer to a path``() =
    [<Test>]
    member x.``when a single buffer exists``() =
        ()