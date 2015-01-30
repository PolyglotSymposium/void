namespace Void.ViewModel

type RGBColor = {
    Red : byte
    Green : byte
    Blue : byte
}

type Colorscheme = {
    Background : RGBColor
    Foreground : RGBColor
    DimForeground : RGBColor
    WindowDecorations : RGBColor
    Error : RGBColor
}

module Colors =
    let white = { Red = 255uy; Green = 255uy; Blue = 255uy }
    let lightGray = { Red = 192uy; Green = 192uy; Blue = 192uy }
    let black = { Red = 0uy; Green = 0uy; Blue = 0uy }
    let red = { Red = 255uy; Green = 0uy; Blue = 0uy }
    let green = { Red = 0uy; Green = 255uy; Blue = 0uy }
    let blue = { Red = 0uy; Green = 0uy; Blue = 255uy }

    let defaultColorscheme = {
        Background = black
        Foreground = white
        DimForeground = lightGray
        WindowDecorations = blue
        Error = red
    }


