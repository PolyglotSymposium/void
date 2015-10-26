namespace Void.Core

type BufferMessage = inherit Message

type BufferEnvelopeMessage<'TBufferMessage when 'TBufferMessage :> BufferMessage> =
    {
        BufferId : int
        Message : 'TBufferMessage
    }
    interface EnvelopeMessage<'TBufferMessage>

type FileBufferProxy = {
    MaybeFilepath : string option
    Contents : string seq
}

type MoveCursor<'By> =
    | MoveCursor of Move<'By>
    interface CommandMessage
    interface BufferMessage

type MoveCursorTo<'By, 'In> =
    | MoveCursorTo of MoveTo<'By, 'In>
    interface CommandMessage
    interface BufferMessage

[<RequireQualifiedAccess>]
type BufferEvent =
    | Added of FileBufferProxy
    | CursorMoved of From : CellGrid.Cell * To : CellGrid.Cell
    interface EventMessage
    interface BufferMessage

type GetBufferContentsRequest =
    {
        StartingAtLine : int<mLine>
    }
    interface RequestMessage
    interface BufferMessage

type GetBufferContentsResponse =
    {
        FirstLineNumber : int<mLine>
        RequestedContents : string seq
    }
    interface ResponseMessage<GetBufferContentsRequest>
    interface BufferMessage
