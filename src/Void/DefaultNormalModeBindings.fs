namespace Void

module DefaultNormalModeBindings = 
    open Void.Core

    let commonBindings =
        [
            [KeyPress.G; KeyPress.Q; KeyPress.Q], CoreCommand.FormatCurrentLine :> CommandMessage
            [KeyPress.ControlL], CoreCommand.Redraw :> CommandMessage
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
