using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using Void.Core;
using Void.ViewModel;

namespace Void
{
    public class MainForm : Form, MainView
    {
        private readonly MainController _controller;
        private Font _font;
        private float _lineHeight;

        public MainForm()
        {
            _controller = new MainController(this);
            _controller.init();
            PopulateContent(25); // TODO do not hard-code
        }

        public void SubscribeToKeyUp(Action<KeyPress> handler)
        {
            KeyUp += (sender, eventArgs) =>
            {
                var keyPress = eventArgs.AsVoidKeyPress();
                if (keyPress != null)
                {
                    handler(keyPress);
                }
            };
        }

        public FontMetrics GetFontMetrics()
        {
            var verticalPadding = Platform.IsGtk ? 0 : 3;
            var horizontalPadding = Platform.IsGtk ? 0.0 : 2.77;
            var height = _font.LineHeight + verticalPadding;
            var width = _font.XHeight + horizontalPadding;
            _lineHeight = height; // TODO DELETE
            return new FontMetrics(height, width);
        }

        public void SetBackgroundColor(RGBColor color)
        {
            BackgroundColor = color.AsEtoColor();
        }

        public void SetFontBySize(byte size)
        {
            _font = Fonts.Monospace(size);
        }

        public void SetViewSize(SizeInPixels size)
        {
            ClientSize = size.AsEtoSize();
        }

        public void SetViewTitle(string title)
        {
            Title = title;
        }

        private void PopulateContent(int numberOfRows)
        {
            var drawable = new Drawable();
            drawable.Paint += (sender, pe) =>
            {
                var brush = new SolidBrush(_controller.foregroundColor().AsEtoColor());
                Action<PointF, string> draw = (pointf, text) => pe.Graphics.DrawText(_font, brush, pointf, text);
                draw(new PointF(2f, 0f), "XWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWX");
                foreach (var i in Enumerable.Range(1, numberOfRows-2))
                {
                    var offset = _lineHeight*i;
                    draw(new PointF(2f, offset), ("X Line #" + i));
                }
                var offset25 = _lineHeight*(numberOfRows-1);
                draw(new PointF(2f, offset25), "XWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWX");
            };
            Content = drawable;
        }
    }
}
