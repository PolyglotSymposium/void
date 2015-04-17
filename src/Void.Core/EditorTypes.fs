namespace Void.Core

[<RequireQualifiedAccess>]
type FileIdentifier = // TODO this is very sketchy right now
    | BufferNumber of int // #1, #2 etc
    | AlternateBuffer // #
    | CurrentBuffer // %
    | Path of string

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

type ModeChange = {
    From : Mode
    To : Mode
}

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

    let lessRows n dimensions =
        { dimensions with Rows = dimensions.Rows - n }

    let lessRowsBelow n block =
        { block with Dimensions = lessRows n block.Dimensions}

// TODO This is naive, obviously
type FileBuffer = private {
    Filepath : string option
    Contents : string list
    CursorPosition : CellGrid.Cell
}

[<RequireQualifiedAccess>]
type BufferType =
    | File of FileBuffer
    | Scratch
    | Shell

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

type Window = {
    CurrentBuffer : int
}

type Tab = {
    Windows : Window list
}

type EditorLayout = {
    Tabs : Tab list
}

type EditorState = {
    CurrentBuffer : int
    BufferList : BufferType list
}
