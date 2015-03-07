namespace Void.Core

// TODO be very careful to get the abstractions right here!
// TODO could be very easy to shoot oneself in the foot with the wrong abstraction!
[<RequireQualifiedAccess>]
type Mode =
    | Insert
    | Normal
    | Command
    | Visual
    | VisualBlock // TODO should this be subsumed under Visual?
    | OperatorPending
    // TODO there are many more modes

module CellGrid =
    type Cell = {
        Row : int
        Column : int
    }
    type Dimensions = {
        Rows : int
        Columns : int
    }
    type Block = {
        UpperLeftCell : Cell
        Dimensions : Dimensions
    }
    let originCell = { Row = 0; Column = 0 }

    let rightOf cell count =
        { Row = cell.Row; Column = cell.Column + count }

    let below cell count =
        { Row = cell.Row + count; Column = cell.Column }

    let lastRow block =
        block.Dimensions.Rows - 1

// TODO This is naive, obviously
type FileBuffer = {
    Filepath : string option
    Contents : string list
    CursorPosition : CellGrid.Cell
}

[<RequireQualifiedAccess>]
type BufferType =
    | File of FileBuffer
    | Scratch
    | Shell
