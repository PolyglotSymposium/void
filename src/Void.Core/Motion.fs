namespace Void.Core

module By =
    (* Unit of measure types seem to get erased at runtime.
     * When we need a downcast to work, we need a type that doesn't get erased.
     * Thus these By* types provide a wrapper. *)
    type Column = Column of int<mColumn>
    type Row = Row of int<mRow>
    type Character = Character of int<mCharacter>
    type Line = Line of int<mLine>

    let row (x : int) =
        Row (x*1<mRow>)

    let column (x : int) =
        Column (x*1<mColumn>)

    let character (x : int) =
        Character (x * 1<mCharacter>)

    let line (x : int) =
        Line (x * 1<mLine>)

module In =
    type Line = private | Line
    type Buffer = private | Buffer

[<RequireQualifiedAccess>]
type Move<'By> = // Relative motion
    | Backward of 'By
    | Forward of 'By

module Move =
    let backward by x =
        Move.Backward (by x)
    let forward by x =
        Move.Forward (by x)

[<RequireQualifiedAccess>]
type MoveTo<'InnerUnit, 'OuterUnit> = // Absolute motion
    | First
    | Nth of 'InnerUnit
    | Last
