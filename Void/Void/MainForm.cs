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

        public MainForm()
        {
            _drawable = new Drawable();
            Content = _drawable;
            _controller = new MainController(this);
            _controller.initializeVoid();
        }

        public PixelGrid.FontMetrics GetFontMetrics()
        {
            var verticalPadding = Platform.IsGtk ? 0 : 3;
            var horizontalPadding = Platform.IsGtk ? 0 : 2;
            var height = Convert.ToUInt16(Math.Ceiling(_font.LineHeight + verticalPadding));
            var width = Convert.ToUInt16(Math.Ceiling(_font.XHeight + horizontalPadding));
            return new PixelGrid.FontMetrics(height, width);
        }

        public void SetBackgroundColor(RGBColor color)
        {
            BackgroundColor = color.AsEtoColor();
        }

        public void SetFontBySize(byte size)
        {
            _font = Fonts.Monospace(size);
        }

        public void SetViewSize(PixelGrid.Dimensions size)
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
    }
}
