namespace Void.Core

[<Measure>] type mCharacter
[<Measure>] type mLine
[<Measure>] type mPararagraph
[<Measure>] type mBuffer

type Motion = interface end

[<RequireQualifiedAccess>]
type Move<[<Measure>]'UnitOfMotion> = // Relative motion
    | Backward of int<'UnitOfMotion>
    | Forward of int<'UnitOfMotion>
    interface Motion

[<RequireQualifiedAccess>]
type MoveTo<[<Measure>]'InnerUnit, [<Measure>]'OuterUnit> = // Absolute motion
    | First
    | Nth of int<'InnerUnit>
    | Last
    interface Motion
