namespace Void.Core

module Messages =
    let private addMessage msg messages = 
        (msg :: messages), Event.MessageAdded msg
    let addError error messages =
        let msg = Message.Error error
        in addMessage msg messages
    let addOutput text messages =
        let msg = Message.Output text
        in addMessage msg messages

type DataOperations = 
    | Append
    | Insert
    | Change
    | Delete
    | Left
    | Right
    | Up
    | Down
    | Escape // TODO Is there a better word for this?

type Coordinate = {
    Row : uint32
    Column : uint32
}

type ScreenBuffer = {
    Buffer : Buffer
    ScreenPosition : Coordinate
    BufferPosition : Coordinate
}

type EditorState = {
    CurrentBuffer : ScreenBuffer
    BufferList : Buffer list
    Mode : Mode
}

module Editor = 
    let private startOfFile() =
        { Row = 0u; Column = 0u }

    let testFileBuffer() =
        let text = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX\n\
                    X Line #1                                                                      X\n\
                    X Line #2                                                                      X\n\
                    X Line #3                                                                      X\n\
                    X Line #4                                                                      X\n\
                    X Line #5                                                                      X\n\
                    X Line #6                                                                      X\n\
                    X Line #7                                                                      X\n\
                    X Line #8                                                                      X\n\
                    X Line #9                                                                      X\n\
                    X Line #10                                                                     X\n\
                    X Line #11                                                                     X\n\
                    X Line #12                                                                     X\n\
                    X Line #13                                                                     X\n\
                    X Line #14                                                                     X\n\
                    X Line #15                                                                     X\n\
                    X Line #16                                                                     X\n\
                    X Line #17                                                                     X\n\
                    X Line #18                                                                     X\n\
                    X Line #19                                                                     X\n\
                    X Line #20                                                                     X\n\
                    X Line #21                                                                     X\n\
                    X Line #22                                                                     X\n\
                    X Line #23                                                                     X\n\
                    XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
        Buffer.File { Filepath = None; Contents = text.Split [|'\n'|] |> Array.toList }

    let private emptyScreenBuffer() =
        {
            Buffer = Buffer.Empty
            ScreenPosition = startOfFile()
            BufferPosition = startOfFile()
        }

    let init (commands : CommandLine.Arguments) =
        let currentBuffer = emptyScreenBuffer()
        {
            CurrentBuffer = currentBuffer
            BufferList = [currentBuffer.Buffer]
            Mode = Mode.Normal
        }

    let readLines buffer start =
        match buffer with
        | Buffer.File fileBuffer ->
            fileBuffer.Contents |> Seq.skip (start - 1) // Line numbers start at 1
        | _ -> Seq.empty
