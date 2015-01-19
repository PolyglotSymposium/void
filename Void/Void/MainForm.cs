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
        private Drawable _drawable;
        private Font _font;
        private float _lineHeight;

        public MainForm()
        {
            _controller = new MainController(this);
            _controller.init();
            PopulateContent(25); // TODO do not hard-code
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

        public void SubscribeToDraw(Action<Artist> handler)
        {
            _drawable.Paint += (sender, eventArgs) =>
            {
                handler(new EtoArtist(eventArgs.Graphics, _font));
            };
        }

        public void Redraw()
        {
            _drawable.Update(new Rectangle(ClientSize));
        }

        private void PopulateContent(int numberOfRows)
        {
            _drawable = new Drawable();
            _drawable.Paint += (sender, eventArgs) =>
            {
                var artist = new EtoArtist(eventArgs.Graphics, _font);
                var offset25 = _lineHeight*(numberOfRows-1);
                artist.RenderText("XWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWX", new View.Point(2f, 0f), Void.ViewModel.Colors.white);
                foreach (var i in Enumerable.Range(1, numberOfRows-2))
                {
                    var offset = _lineHeight*i;
                    artist.RenderText("X Line #" + i, new View.Point(2f, offset), Void.ViewModel.Colors.white);
                }
                artist.RenderText("XWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWX", new View.Point(2f, offset25), Void.ViewModel.Colors.white);
            };
            Content = _drawable;
        }
    }
}
