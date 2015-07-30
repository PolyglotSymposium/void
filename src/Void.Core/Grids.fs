﻿namespace Void.Core

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

    let vectorAdd cell1 cell2 =
        {
            Row = cell1.Row + cell2.Row
            Column = cell1.Column + cell2.Column
        }

module GridConvert =
    open PointGrid
    open CellGrid

    let upperLeftCornerOf cell =
        { X = cell.Column; Y = cell.Row }

    let dimensionsInPoints dimensions =
        { Width = dimensions.Columns; Height = dimensions.Rows }

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