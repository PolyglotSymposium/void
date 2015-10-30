namespace Void.ViewModel

open Void.Core

type WindowMessage = inherit Message

type GetWindowContentsRequest =
    {
        StartingAtLine : int<mLine>
    }
    interface RequestMessage
    interface WindowMessage

type GetWindowContentsResponse =
    {
        FirstLineNumber : int<mLine>
        RequestedContents : string seq
    }
    interface ResponseMessage<GetWindowContentsRequest>
    interface WindowMessage

type WindowEnvelopeMessage<'TBufferMessage when 'TBufferMessage :> BufferMessage> =
    {
        WindowId : int
        Message : 'TBufferMessage
    }
    interface EnvelopeMessage<'TBufferMessage>

type CurrentBufferCommandEnvelope<'TBufferMessage when 'TBufferMessage :> BufferMessage> =
    | InCurrentBuffer of 'TBufferMessage
    interface EnvelopeMessage<'TBufferMessage>
    interface CommandMessage
