namespace Void.ViewModel

type WindowDimensions = {
    Rows : uint16
    Columns : uint16
}

type RGBColor = {
    Red : byte
    Green : byte
    Blue : byte
}

module Colors =
    let white = { Red = 255uy; Green = 255uy; Blue = 255uy }
    let black = { Red = 0uy; Green = 0uy; Blue = 0uy }

type VoidController() =
    member x.startupDimensions() =
        { Rows = 25us; Columns = 80us }
    member x.backgroundColor() =
        Colors.black
    member x.foregroundColor() =
        Colors.white
    member x.titlebarText() =
        "Void - A text editor in the spirit of Vim"