using System.Drawing;
using Void.ViewModel;

namespace Void.UI
{
    public class WinFormsArtist
    {
        private readonly Graphics _graphics;
        private readonly Font _font;
        private readonly CellMetrics _cellMetrics;

        public WinFormsArtist(Graphics graphics, Font font, CellMetrics cellMetrics)
        {
            _graphics = graphics;
            _font = font;
            _cellMetrics = cellMetrics;
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
            _graphics.DrawString(drawing.Text, _font, drawing.Color.AsWinFormsSolidBrush(), drawing.UpperLeftCorner.AsWinFormsPointF(_cellMetrics));
        }

        private void RenderBlock(ScreenBlockObject drawing)
        {
            _graphics.FillRectangle(drawing.Color.AsWinFormsSolidBrush(), drawing.Area.AsWinFormsRectangleF(_cellMetrics));
        }
    }
}
