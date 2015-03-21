using System;
using System.Drawing;
using System.Windows.Forms;
using Void.ViewModel;
using Microsoft.FSharp.Core;

namespace Void.UI
{
    public partial class MainForm : Form, MainView
    {
        private readonly FSharpFunc<ViewEvent,Unit> _handleViewEvent;
        private Font _font = new Font(FontFamily.GenericMonospace, 9);

        public MainForm()
        {
            InitializeComponent();
            _handleViewEvent = Init.initializeVoid(this);
            KeyUp += (sender, eventArgs) =>
            {
                var keyPress = eventArgs.AsVoidKeyPress();
                if (keyPress == null)
                {
                    Console.WriteLine("Warning: failed to translate key stroke");
                }
                else
                {
                    _handleViewEvent.Invoke(ViewEvent.NewKeyPressed(keyPress));
                }
            };
            Paint += (sender, eventArgs) => _handleViewEvent.Invoke(ViewEvent.NewPaintInitiated(new WinFormsArtist(eventArgs.Graphics, _font).Draw));
        }

        public PixelGrid.FontMetrics GetFontMetrics()
        {
            return new PixelGrid.FontMetrics(_font.Height, MeasureFontWidth());
        }

        private int MeasureFontWidth()
        {
            // TODO this isn't working 100% well
            // Ironically, it seems to work better on Mono than .NET
            return Convert.ToInt32(Math.Ceiling(CreateGraphics().MeasureString(new string('X', 80), _font).Width / 80));
        }

        public void SetBackgroundColor(RGBColor color)
        {
            BackColor = color.AsWinFormsColor();
        }

        public void SetFontBySize(byte size)
        {
            _font = new Font(FontFamily.GenericMonospace, 9);
        }

        public void SetViewSize(PixelGrid.Dimensions size)
        {
            ClientSize = size.AsWinFormsSize();
        }

        public void SetViewTitle(string title)
        {
            Text = title;
        }

        public void TriggerDraw(PixelGrid.Block block)
        {
            Invalidate(block.AsWinFormsRectangle());
        }
    }
}
