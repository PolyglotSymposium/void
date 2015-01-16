using System;
using Eto.Drawing;
using Void.ViewModel;

namespace Void
{
    public static class TypeTranslationExtensions
    {
        public static Color AsEtoColor(this RGBColor color)
        {
            return Color.FromArgb(color.Red, color.Green, color.Blue);
        }

        public static Size AsEtoSize(this SizeInPixels size)
        {
            return new Size(size.Width, size.Height);
        }
    }
}
