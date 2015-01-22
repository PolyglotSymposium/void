namespace Void.ViewModel

type RGBColor = {
    Red : byte
    Green : byte
    Blue : byte
}

type Colorscheme = {
    Background : RGBColor
    Foreground : RGBColor
    StatusLineEtc : RGBColor // TODO rename
    Error : RGBColor
}

module Colors =
    let white = { Red = 255uy; Green = 255uy; Blue = 255uy }
    let black = { Red = 0uy; Green = 0uy; Blue = 0uy }
    let red = { Red = 255uy; Green = 0uy; Blue = 0uy }
    let green = { Red = 0uy; Green = 255uy; Blue = 0uy }
    let blue = { Red = 0uy; Green = 0uy; Blue = 255uy }

    let defaultColorscheme = {
        Background = black
        Foreground = white
        StatusLineEtc = blue
        Error = red
    }


