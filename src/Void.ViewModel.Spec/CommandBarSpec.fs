namespace Void.ViewModel.Spec

open Void.Core
open Void.ViewModel
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``The command mode command line (or command bar for short)``() = 
    [<Test>]
    member x.``should display when entering command mode``() =
        let bar, msg = Event.ModeChanged { From = Mode.Normal; To = Mode.Command }
                       |> CommandBarV2.handleEvent
        bar |> should equal CommandBarV2.visibleButEmpty
        msg |> should equal (VMEvent.CommandBar_Displayed bar)
