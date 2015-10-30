namespace Void.Core

module PointGrid =
    type Point = {
        X : int
        Y : int
    }
    type Dimensions = {
        Height : int
        Width : int
    }
    type Block = {
        UpperLeftCorner : Point
        Dimensions : Dimensions
    }

    let originPoint = { X = 0; Y = 0 }

module CellGrid =
    type Cell = {
        Row : int<mRow>
        Column : int<mColumn>
    }
    type Dimensions = {
        Rows : int<mRow>
        Columns : int<mColumn>
    }
    type Block = {
        UpperLeftCell : Cell
        Dimensions : Dimensions
    }
    let originCell = { Row = 0<mRow>; Column = 0<mColumn> }
    let zeroDimensions = { Rows = 0<mRow>; Columns = 0<mColumn> }
    let zeroBlock = { UpperLeftCell = originCell; Dimensions = zeroDimensions }

    let rightOf cell count =
        { Row = cell.Row; Column = cell.Column + count }

    let leftOf cell count =
        { Row = cell.Row; Column = cell.Column - count }

    let below cell count =
        { Row = cell.Row + count; Column = cell.Column }

    let above cell count =
        { Row = cell.Row - count; Column = cell.Column }

    let lastRow block =
        block.Dimensions.Rows - 1<mRow>

    let lessRows n dimensions =
        { dimensions with Rows = dimensions.Rows - n }

    let lessRowsBelow n block =
        { block with Dimensions = lessRows n block.Dimensions}

    let vectorAdd cell1 cell2 =
        {
            Row = cell1.Row + cell2.Row
            Column = cell1.Column + cell2.Column
        }

module GridConvert =
    open PointGrid
    open CellGrid

    let upperLeftCornerOf cell =
        { X = cell.Column/1<mColumn>; Y = cell.Row/1<mRow> }

    let dimensionsInPoints dimensions =
        { Width = dimensions.Columns/1<mColumn>; Height = dimensions.Rows/1<mRow> }

    let boxAround block =
        {
            UpperLeftCorner = upperLeftCornerOf block.UpperLeftCell
            Dimensions = dimensionsInPoints block.Dimensions
        }

    let boxAroundOneCell cell =
        {
            UpperLeftCorner = upperLeftCornerOf cell
            Dimensions = { Width = 1; Height = 1 }
        }
