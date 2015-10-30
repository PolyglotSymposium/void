namespace Void.ViewModel

module Sizing =
    open Void.Core
    open Void.Core.CellGrid
    let defaultViewSize = { Rows = 26<mRow>; Columns = 80<mColumn> }
    let defaultViewArea = { UpperLeftCell = originCell; Dimensions = defaultViewSize }

module ViewModel = // TODO name suspect at this point
    open Void.Util
    open Void.Core
    open Void.Core.CellGrid

    let defaultTitle = "Void - A text editor in the spirit of Vim"
    let defaultFontSize = 9

    let defaultViewModel =
        {
            Size = Sizing.defaultViewSize
            Title = defaultTitle
            BackgroundColor = Colors.defaultColorscheme.Background
            FontSize = defaultFontSize
        }

    let wholeArea view =
        {
            UpperLeftCell = originCell
            Dimensions = view.Size
        }

    let upperLeftCellOfCommandBar viewModel =
        // TODO this is just hacked together for the moment
        { Row = CellGrid.lastRow (wholeArea viewModel); Column = 0<mColumn> }

    let areaOfCommandBarOrNotifications viewModel =
        // TODO this is just hacked together for the moment
        {
            UpperLeftCell = upperLeftCellOfCommandBar viewModel
            Dimensions = { Rows = 1<mRow>; Columns = viewModel.Size.Columns }
        }
