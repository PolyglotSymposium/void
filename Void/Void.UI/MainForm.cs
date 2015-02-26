using System;
using System.Drawing;
using System.Windows.Forms;
using Void.ViewModel;

namespace Void.UI
{
    public class MainForm : Form, MainView
    {
        private readonly MainController _controller;
        private readonly Drawable _drawable;
        private Font _font = Font.Monospace(9);

        public MainForm()
        {
            _drawable = new Drawable();
            Content = _drawable;
            _controller = new MainController(this);
            _controller.initializeVoid();
            KeyUp += (sender, eventArgs) =>
            {
                var keyPress = eventArgs.AsVoidKeyPress();
                if (keyPress != null)
                {
                    _controller.handleViewEvent(ViewEvent.NewKeyPressed(keyPress));
                }
            };
            _drawable.Paint += (sender, eventArgs) =>
            {
                _controller.handleViewEvent(ViewEvent.NewPaintInitiated(new WinFormsArtist(eventArgs.Graphics, _font).Draw));
            };
        }

        public PixelGrid.FontMetrics GetFontMetrics()
        {
            var height = Convert.ToUInt16(Math.Ceiling(_font.LineHeight + 3));
            var width = Convert.ToUInt16(Math.Ceiling(_font.XHeight + 3));
            return new PixelGrid.FontMetrics(height, width);
        }

        public void SetBackgroundColor(RGBColor color)
        {
            BackgroundColor = color.AsWinFormsColor();
        }

        public void SetFontBySize(byte size)
        {
            _font = Fonts.Monospace(size);
        }

        public void SetViewSize(PixelGrid.Dimensions size)
        {
            ClientSize = size.AsWinFormsSize();
        }

        public void SetViewTitle(string title)
        {
            Title = title;
        }

        public void TriggerDraw()
        {
            _drawable.Update(new Rectangle(ClientSize));
        }
    }
}
