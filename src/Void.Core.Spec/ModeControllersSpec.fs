namespace Void.ViewModel.Spec

open Void.Core
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Normal mode input handler``() = 
    let handleAndVerifyZQ (handler : NormalModeInputHandler) = 
        handler.handleKeyPress KeyPress.ShiftZ |> should equal Command.Noop
        handler.handleKeyPress KeyPress.ShiftQ |> should equal Command.QuitWithoutSaving

    [<Test>]
    member x.``should return the appropriate command once a mapping is completed``() =
        NormalModeInputHandler() |> handleAndVerifyZQ

    [<Test>]
    member x.``should clear out buffered keys once a mapping is completed``() =
        let controller = NormalModeInputHandler()
        handleAndVerifyZQ controller
        handleAndVerifyZQ controller
