namespace Void.Core

[<RequireQualifiedAccess>]
type Move<[<Measure>]'UnitOfMotion> = // Relative motion
    | Backward of int<'UnitOfMotion>
    | Forward of int<'UnitOfMotion>

[<RequireQualifiedAccess>]
type MoveTo<[<Measure>]'InnerUnit, [<Measure>]'OuterUnit> = // Absolute motion
    | First
    | Nth of int<'InnerUnit>
    | Last
