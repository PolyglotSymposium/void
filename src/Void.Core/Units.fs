namespace Void.Core

[<Measure>] type mCharacter
[<Measure>] type mLine
[<Measure>] type mPararagraph
[<Measure>] type mBuffer

[<Measure>] type mRow
[<Measure>] type mColumn

[<AutoOpen>]
module UnitConversions =
    let linePerRow = 1<mLine/mRow>
    let rowPerLine = 1/linePerRow

    (* IMPORTANT!!! Lines are 1-based. Rows are 0-based. *)

    let ``line#->row#`` ``line#`` =
        ``line#`` * rowPerLine - 1<mRow>

    let ``row#->line#`` ``row#`` =
        ``row#`` * linePerRow + 1<mLine>
