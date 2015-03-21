namespace Void.ViewModel.Spec

open Void.Core
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Normal mode controller``() = 
    let handleAndVerifyZQ (controller : NormalModeController) = 
        controller.handleKeyPress KeyPress.ShiftZ |> should equal Command.Noop
        controller.handleKeyPress KeyPress.ShiftQ |> should equal Command.Quit

    [<Test>]
    member x.``should return the appropriate command once a mapping is completed``() =
        NormalModeController() |> handleAndVerifyZQ

    [<Test>]
    member x.``should clear out buffered keys once a mapping is completed``() =
        let controller = NormalModeController()
        handleAndVerifyZQ controller
        handleAndVerifyZQ controller
