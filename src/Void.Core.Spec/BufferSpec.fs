namespace Void.Core.Spec

open Void.Core
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Moving the cursor in a buffer``() = 
    let h = Move.Backward 1<mCharacter> :> Motion |> BufferCommand.MoveCursor
    let j = Move.Forward 1<mLine> :> Motion |> BufferCommand.MoveCursor
    let k = Move.Backward 1<mLine> :> Motion |> BufferCommand.MoveCursor
    let l = Move.Forward 1<mCharacter> :> Motion |> BufferCommand.MoveCursor

    let oneLineBuffer = Buffer.prepend Buffer.emptyFile "foobar"
    let oneCharacterBuffer = Buffer.prepend Buffer.emptyFile "o"

    [<Test>]
    member x.``down one line should do nothing in an empty buffer``() =
        Buffer.handleCommand Buffer.emptyFile j
        |> should equal (Buffer.emptyFile, noMessage)

    [<Test>]
    member x.``up one line should do nothing in an empty buffer``() =
        Buffer.handleCommand Buffer.emptyFile k
        |> should equal (Buffer.emptyFile, noMessage)

    [<Test>]
    member x.``backward one character should do nothing in an empty buffer``() =
        Buffer.handleCommand Buffer.emptyFile h
        |> should equal (Buffer.emptyFile, noMessage)

    [<Test>]
    member x.``forward one character should do nothing in an empty buffer``() =
        Buffer.handleCommand Buffer.emptyFile l
        |> should equal (Buffer.emptyFile, noMessage)

    [<Test>]
    member x.``down one line should do nothing in a buffer with only one line``() =
        Buffer.handleCommand oneLineBuffer j
        |> should equal (oneLineBuffer, noMessage)

    [<Test>]
    member x.``up one line should do nothing in a buffer with only one line``() =
        Buffer.handleCommand oneLineBuffer k
        |> should equal (oneLineBuffer, noMessage)

    [<Test>]
    member x.``backward one character should do nothing in a one-character buffer``() =
        Buffer.handleCommand oneCharacterBuffer h
        |> should equal (oneCharacterBuffer, noMessage)

    [<Test>]
    member x.``forward one character should do nothing in a one-character buffer``() =
        Buffer.handleCommand oneCharacterBuffer l
        |> should equal (oneCharacterBuffer, noMessage)
