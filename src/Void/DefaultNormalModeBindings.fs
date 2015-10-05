namespace Void

module DefaultNormalModeBindings = 
    open Void.Core
    open Void.ViewModel

    let move<'TBy, [<Measure>]'TUom> movement =
        (MoveCursor movement : MoveCursor<'TBy, 'TUom>)
        |> InCurrentBuffer :> CommandMessage
    let moveTo place =
        MoveCursorTo place
        |> InCurrentBuffer :> CommandMessage

    let commonBindings =
        [
            [KeyPress.H], move<ByColumn, mColumn> (Move.Backward 1<mColumn>)
            [KeyPress.J], move<ByRow, mRow> (Move.Forward 1<mRow>)
            [KeyPress.K], move<ByRow, mRow> (Move.Backward 1<mRow>)
            [KeyPress.L], move<ByColumn, mColumn> (Move.Forward 1<mColumn>)

            [KeyPress.Zero], moveTo MoveTo<mCharacter,mLine>.First
            [KeyPress.DollarSign], moveTo MoveTo<mCharacter,mLine>.Last

            [KeyPress.G; KeyPress.G], moveTo MoveTo<mLine,mBuffer>.First
            [KeyPress.ShiftG], moveTo MoveTo<mLine,mBuffer>.Last

            [KeyPress.G; KeyPress.Q; KeyPress.G; KeyPress.Q], CoreCommand.FormatCurrentLine :> CommandMessage // TODO this is an abomination of course

            [KeyPress.ControlL], CoreCommand.Redraw :> CommandMessage

            [KeyPress.ControlD], VMCommand.ScrollHalf (Move.Forward 1<mScreenHeight>) :> CommandMessage
            [KeyPress.ControlU], VMCommand.ScrollHalf (Move.Backward 1<mScreenHeight>) :> CommandMessage
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
        Seq.map NormalModeBindings.Command.Bind bindings

    let handleCommand (bus : Bus) command =
        match command with
        | CoreCommand.InitializeVoid ->
            for message in bindAllCommands voidBindings do
                bus.publish message
        | _ -> ()
        noMessage


    module Service =
        let subscribe (bus : Bus) =
            bus.subscribe (handleCommand bus)
