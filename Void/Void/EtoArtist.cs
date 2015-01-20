using Eto.Drawing;
using Void.ViewModel;

namespace Void
{
    public class EtoArtist : Artist
    {
        private readonly Graphics _graphics;
        private readonly Font _font;

        public EtoArtist(Graphics graphics, Font font)
        {
            _graphics = graphics;
            _font = font;
        }

        public void RenderText(string text, PixelGrid.Point startingPoint, RGBColor color)
        {
            _graphics.DrawText(_font, color.AsEtoColor(), startingPoint.AsEtoPointF(), text);
        }

        public void RenderBlock(PixelGrid.Block block, RGBColor color)
        {
            _graphics.FillRectangle(color.AsEtoColor(), block.AsEtoRectangleF());
        }
    }
}
