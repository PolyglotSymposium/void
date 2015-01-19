namespace Void.ViewModel.Spec

open Void.Core
open Void.ViewModel
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Normal mode controller``() = 
    let mutable invoked = false
    [<Test>]
    member x.``should call the command handler for the command which the binding is mapped to``() =
        let handleCommand command =
            invoked <- match command with
                       | Command.Quit -> true
                       | _ -> false
            ()
        let controller = NormalModeController()
        controller.handle KeyPress.ShiftZ handleCommand
        invoked |> should be False
        controller.handle KeyPress.ShiftQ handleCommand
        invoked |> should be True
