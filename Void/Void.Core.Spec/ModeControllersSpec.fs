namespace Void.ViewModel.Spec

open Void.Core
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Normal mode controller``() = 
    [<Test>]
    member x.``should return the appropriate command once a mapping is completed``() =
        let controller = NormalModeController()
        controller.handle KeyPress.ShiftZ |> should equal Command.Noop
        controller.handle KeyPress.ShiftQ |> should equal Command.Quit
