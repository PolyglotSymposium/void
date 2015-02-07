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

        public void RenderText(ScreenTextObject obj)
        {
            _graphics.DrawText(_font, obj.Color.AsEtoColor(), obj.UpperLeftCorner.AsEtoPointF(), obj.Text);
        }

        public void RenderBlock(ScreenBlockObject obj)
        {
            _graphics.FillRectangle(obj.Color.AsEtoColor(), obj.Area.AsEtoRectangleF());
        }
    }
}
