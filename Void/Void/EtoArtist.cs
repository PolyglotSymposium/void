using Eto.Drawing;
using Void.ViewModel;

namespace Void
{
    public class EtoArtist
    {
        private readonly Graphics _graphics;
        private readonly Font _font;

        public EtoArtist(Graphics graphics, Font font)
        {
            _graphics = graphics;
            _font = font;
        }

        public void Draw(DrawingObject drawing)
        {
            if (drawing.IsLine)
            {
                RenderLine(drawing.AsLine());
            }
            else if (drawing.IsText)
            {
                RenderText(drawing.AsText());
            }
            else if (drawing.IsBlock)
            {
                RenderBlock(drawing.AsBlock());
            }
        }

        private void RenderLine(ScreenLineObject drawing)
        {
            throw new System.NotImplementedException();
        }

        private void RenderText(ScreenTextObject drawing)
        {
            _graphics.DrawText(_font, drawing.Color.AsEtoColor(), drawing.UpperLeftCorner.AsEtoPointF(), drawing.Text);
        }

        private void RenderBlock(ScreenBlockObject drawing)
        {
            _graphics.FillRectangle(drawing.Color.AsEtoColor(), drawing.Area.AsEtoRectangleF());
        }
    }
}
