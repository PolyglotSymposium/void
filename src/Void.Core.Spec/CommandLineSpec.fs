namespace Void.Core.Spec

open Void.Core.CommandLine
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Parsing the command line arguments``() = 
    [<Test>]
    member x.``when there are no arguments``() =
        parseArgs [] |> should equal (ParseSucceeded { FilePaths = [] })

    [<Test>]
    member x.``when there is one argument``() =
        parseArgs ["myfile.txt"] |> should equal (ParseSucceeded { FilePaths = ["myfile.txt"] })
