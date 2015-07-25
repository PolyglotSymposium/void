namespace Void.Core
open System.Collections.Generic

type NormalBindings = Map<KeyPress list, CommandMessage>

type KeyPressed =
    {
        Key : KeyPress
        Timestamp : System.DateTime
    }
    interface EventMessage

module NormalMode =
    [<RequireQualifiedAccess>]
    type Command =
        | Bind of KeyPress list * CommandMessage
        interface CommandMessage

    [<RequireQualifiedAccess>]
    type Event =
        | KeysBoundToCommand of KeyPress list * CommandMessage
        | KeyPressRegistered of KeyPress
        | KeyPressesCleared
        | ExpiredKeyPressReplacedWith of KeyPress
        interface EventMessage

    type State = {
        Bindings : NormalBindings
        KeyPressBuffer : KeyPress list
        ExpireTime : System.DateTime
    }

    let empty =
        {
            Bindings = Map.empty<KeyPress list, CommandMessage>
            KeyPressBuffer = []
            ExpireTime = System.DateTime.MaxValue
        }

    let private update state keyPress keyPresses =
        {
            Bindings = state.Bindings
            KeyPressBuffer = keyPresses
            ExpireTime = keyPress.Timestamp.AddSeconds 1.0
        }
        
    let private reset state =
        {
            Bindings = state.Bindings
            KeyPressBuffer = []
            ExpireTime = empty.ExpireTime
        }

    let private bind state keyPresses command =
        { state with Bindings = state.Bindings.Add(keyPresses, command) }

    let private replaceExpired state keyPress =
        update state keyPress [keyPress.Key], Event.ExpiredKeyPressReplacedWith keyPress.Key :> Message

    let private matched state keyPress keysInBindOrder =
        reset state, state.Bindings.Item keysInBindOrder :> Message

    let private awaitingFurtherKeys state keyPress keyPresses =
        update state keyPress keyPresses, Event.KeyPressRegistered keyPress.Key :> Message

    let matchExpired state keyPress =
        let keyPresses = [keyPress.Key]
        if state.Bindings.ContainsKey keyPresses
        then matched state keyPress keyPresses
        else replaceExpired state keyPress

    let private matchUnexpired state keyPress =
        let keyPresses = keyPress.Key :: state.KeyPressBuffer
        let inBindOrder = List.rev keyPresses
        if state.Bindings.ContainsKey inBindOrder
        then matched state keyPress inBindOrder
        else awaitingFurtherKeys state keyPress keyPresses

    let handleKeyPress state keyPress =
        if keyPress.Key = KeyPress.Escape then
            reset state, Event.KeyPressesCleared :> Message
        else
            if keyPress.Timestamp < state.ExpireTime
            then matchUnexpired state keyPress
            else matchExpired state keyPress

    let handleCommand state (Command.Bind (keyPresses, bindToCommand)) =
        bind state keyPresses bindToCommand, Event.KeysBoundToCommand (keyPresses, bindToCommand) :> Message

    type InputHandler() =
        let _state = ref empty

        member x.handleKeyPress (keyPress : KeyPress) =
            let keyPressed = { Key = keyPress; Timestamp = System.DateTime.Now } // TODO push this up to the view
            in Service.wrap _state handleKeyPress keyPressed

        member x.handleCommand =
            Service.wrap _state handleCommand
