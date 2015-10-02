namespace Void.Core.Spec

open Void.Core
open NUnit.Framework
open FsUnit

[<TestFixture>]
type ``Moving the cursor in a buffer``() = 
    let h = Move.Backward 1<mColumn> |> MoveCursor
    let j = Move.Forward 1<mRow> |> MoveCursor
    let k = Move.Backward 1<mRow> |> MoveCursor
    let l = Move.Forward 1<mColumn> |> MoveCursor

    let ``3j`` = Move.Forward 3<mRow> |> MoveCursor
    let ``3k`` = Move.Backward 3<mRow> |> MoveCursor
    let ``5j`` = Move.Forward 5<mRow> |> MoveCursor
    let ``5k`` = Move.Backward 5<mRow> |> MoveCursor

    let oneLineBuffer = Buffer.prepend Buffer.emptyFile "line 1"
    let twoLineBuffer = Buffer.prepend oneLineBuffer "line 2"
    let fiveLineBuffer =
        ["line 1"; "line 2"; "line 3"; "line 4"; "line 5"]
        |> Buffer.loadContents Buffer.emptyFile
    let oneCharacterBuffer = Buffer.prepend Buffer.emptyFile "1"

    let shouldEqual expected actual =
        printfn "Expected: %A" expected
        printfn "Actual: %A" actual
        should equal expected actual

    [<Test>]
    member x.``down one line should do nothing in an empty buffer``() =
        Buffer.handleMoveCursorByRows Buffer.emptyFile j
        |> should equal (Buffer.emptyFile, noMessage)

    [<Test>]
    member x.``up one line should do nothing in an empty buffer``() =
        Buffer.handleMoveCursorByRows Buffer.emptyFile k
        |> should equal (Buffer.emptyFile, noMessage)

    [<Test>]
    member x.``backward one character should do nothing in an empty buffer``() =
        Buffer.handleMoveCursorByColumns Buffer.emptyFile h
        |> should equal (Buffer.emptyFile, noMessage)

    [<Test>]
    member x.``forward one character should do nothing in an empty buffer``() =
        Buffer.handleMoveCursorByColumns Buffer.emptyFile l
        |> should equal (Buffer.emptyFile, noMessage)

    [<Test>]
    member x.``down one line should do nothing in a buffer with only one line``() =
        Buffer.handleMoveCursorByRows oneLineBuffer j
        |> should equal (oneLineBuffer, noMessage)

    [<Test>]
    member x.``up one line should do nothing in a buffer with only one line``() =
        Buffer.handleMoveCursorByRows oneLineBuffer k
        |> should equal (oneLineBuffer, noMessage)

    [<Test>]
    member x.``backward one character should do nothing in a one-character buffer``() =
        Buffer.handleMoveCursorByColumns oneCharacterBuffer h
        |> should equal (oneCharacterBuffer, noMessage)

    [<Test>]
    member x.``forward one character should do nothing in a one-character buffer``() =
        Buffer.handleMoveCursorByColumns oneCharacterBuffer l
        |> should equal (oneCharacterBuffer, noMessage)

    [<Test>]
    member x.``down one line should succeed within a two-line buffer``() =
        Buffer.handleMoveCursorByRows twoLineBuffer j
        |> snd
        |> should equal (BufferEvent.CursorMoved(CellGrid.originCell, { Column = 0<mColumn>; Row = 1<mRow> }))

    [<Test>]
    member x.``moving up one line should undo moving down one line``() =
        let moved = Buffer.handleMoveCursorByRows twoLineBuffer j |> fst
        Buffer.handleMoveCursorByRows moved k
        |> shouldEqual (twoLineBuffer, BufferEvent.CursorMoved({ Column = 0<mColumn>; Row = 1<mRow> }, CellGrid.originCell) :> Message)

    [<Test>]
    member x.``down three lines should succeed within a five-line buffer``() =
        Buffer.handleMoveCursorByRows fiveLineBuffer ``3j``
        |> snd
        |> should equal (BufferEvent.CursorMoved(CellGrid.originCell, { Column = 0<mColumn>; Row = 3<mRow> }))

    [<Test>]
    member x.``moving up three lines should undo moving down three lines``() =
        let moved = Buffer.handleMoveCursorByRows fiveLineBuffer ``3j`` |> fst
        Buffer.handleMoveCursorByRows moved ``3k``
        |> shouldEqual (fiveLineBuffer, BufferEvent.CursorMoved({ Column = 0<mColumn>; Row = 3<mRow> }, CellGrid.originCell) :> Message)

    [<Test>]
    member x.``trying to move down five lines within a five-line buffer should not overshoot``() =
        Buffer.handleMoveCursorByRows fiveLineBuffer ``5j``
        |> snd
        |> shouldEqual (BufferEvent.CursorMoved(CellGrid.originCell, { Column = 0<mColumn>; Row = 4<mRow> }))

    [<Test>]
    member x.``moving up five lines in a five-line buffer from the bottom should not overshoot``() =
        let moved = Buffer.handleMoveCursorByRows fiveLineBuffer ``5j`` |> fst
        Buffer.handleMoveCursorByRows moved ``5k``
        |> shouldEqual (fiveLineBuffer, BufferEvent.CursorMoved({ Column = 0<mColumn>; Row = 4<mRow> }, CellGrid.originCell) :> Message)
