namespace Void

module DefaultNormalModeBindings = 
    open Void.Core
    open Void.ViewModel

    let move movement =
        MoveCursor movement
        |> InCurrentBuffer :> CommandMessage
    let moveTo place =
        MoveCursorTo place
        |> InCurrentBuffer :> CommandMessage

    let commonBindings =
        [
            [KeyPress.H], move (Move.backward By.column 1)
            [KeyPress.J], move (Move.forward By.row 1)
            [KeyPress.K], move (Move.backward By.row 1)
            [KeyPress.L], move (Move.forward By.column 1)

            [KeyPress.Zero], moveTo MoveTo<mCharacter,mLine>.First
            [KeyPress.DollarSign], moveTo MoveTo<mCharacter,mLine>.Last

            [KeyPress.G; KeyPress.G], moveTo MoveTo<mLine,mBuffer>.First
            [KeyPress.ShiftG], moveTo MoveTo<mLine,mBuffer>.Last

            [KeyPress.G; KeyPress.Q; KeyPress.G; KeyPress.Q], CoreCommand.FormatCurrentLine :> CommandMessage // TODO this is an abomination of course

            [KeyPress.ControlL], CoreCommand.Redraw :> CommandMessage

            [KeyPress.ControlD], VMCommand.ScrollHalf (Move.forward vmBy.screenHeight 1) :> CommandMessage
            [KeyPress.ControlU], VMCommand.ScrollHalf (Move.backward vmBy.screenHeight 1) :> CommandMessage
            [KeyPress.ControlE], VMCommand.Scroll (Move.forward By.line 1) :> CommandMessage
            [KeyPress.ControlY], VMCommand.Scroll (Move.backward By.line 1) :> CommandMessage

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
