namespace Void

module DefaultNormalModeBindings = 
    open Void.Core
    open Void.ViewModel

    let commonBindings =
        [
            [KeyPress.H], VMCommand.Move (Move.Backward 1<mCharacter>) :> CommandMessage
            [KeyPress.J], VMCommand.Move (Move.Forward 1<mLine>) :> CommandMessage
            [KeyPress.K], VMCommand.Move (Move.Backward 1<mLine>) :> CommandMessage
            [KeyPress.L], VMCommand.Move (Move.Forward 1<mCharacter>) :> CommandMessage

            [KeyPress.Zero], VMCommand.Move MoveTo<mCharacter,mLine>.First :> CommandMessage
            [KeyPress.DollarSign], VMCommand.Move MoveTo<mCharacter,mLine>.Last :> CommandMessage

            [KeyPress.G; KeyPress.G], VMCommand.Move MoveTo<mLine,mBuffer>.First :> CommandMessage
            [KeyPress.ShiftG], VMCommand.Move MoveTo<mLine,mBuffer>.Last :> CommandMessage

            [KeyPress.G; KeyPress.Q; KeyPress.Q], CoreCommand.FormatCurrentLine :> CommandMessage // TODO this is an abomination of course

            [KeyPress.ControlL], CoreCommand.Redraw :> CommandMessage

            [KeyPress.ControlE], VMCommand.Scroll (Move.Forward 1<mLine>) :> CommandMessage
            [KeyPress.ControlY], VMCommand.Scroll (Move.Backward 1<mLine>) :> CommandMessage

            [KeyPress.ShiftZ; KeyPress.ShiftQ], CoreCommand.QuitWithoutSaving :> CommandMessage
        ]

    let voidBindings =
        Seq.append commonBindings [
            [KeyPress.Semicolon], CoreCommand.ChangeToMode Mode.Command :> CommandMessage
            [KeyPress.ControlC], CoreCommand.Yank :> CommandMessage
            [KeyPress.ControlV], CoreCommand.Put :> CommandMessage
            [KeyPress.ControlB], CoreCommand.ChangeToMode Mode.VisualBlock :> CommandMessage
            [KeyPress.ShiftZ; KeyPress.ShiftA], CoreCommand.QuitAll :> CommandMessage
        ]

    let vimBindings =
        Seq.append commonBindings [
            [KeyPress.Colon], CoreCommand.ChangeToMode Mode.Command :> CommandMessage
            [KeyPress.ControlV], CoreCommand.ChangeToMode Mode.VisualBlock :> CommandMessage
        ]

    let bindAllCommands bindings =
        Seq.map NormalMode.Command.Bind bindings

    let handleCommand (bus : Bus) command =
        match command with
        | CoreCommand.InitializeVoid ->
            bindAllCommands voidBindings
            |> bus.publishAll
        | _ -> ()
        noMessage


    module Service =
        let subscribe (bus : Bus) =
            bus.subscribe (handleCommand bus)
