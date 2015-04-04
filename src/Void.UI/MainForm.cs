using System;
using System.Drawing;
using System.Windows.Forms;
using Void.Core;
using Void.ViewModel;
using Microsoft.FSharp.Core;

namespace Void.UI
{
    public partial class MainForm : Form, MainView, InputModeChanger
    {
        private Font _font = new Font(FontFamily.GenericMonospace, 9);
        private InputMode<Unit> _inputHandler;

        public MainForm()
        {
            InitializeComponent();
            Init.initializeVoid(this, this);
            KeyUp += (sender, eventArgs) =>
            {
                if (_inputHandler.IsKeyPresses)
                {
                    var keyPress = eventArgs.AsVoidKeyPress();
                    if (keyPress == null)
                    {
                        Console.WriteLine("Warning: failed to translate key stroke");
                    }
                    else
                    {
                        _inputHandler.AsKeyPressesHandler()(keyPress);
                    }
                }
                else
                {
                    // TODO!!! This is a provisional hack; see GitHub issue #2
                    // Should probably be attaching this to a TextInput event or
                    // something instead, but those sort of events don't seem to just
                    // be wired on on the Form level, only in actual text boxes...
                    var textOrHotKey = eventArgs.AsVoidTextOrHotKeyProvisionalHack();
                    if (textOrHotKey == null)
                    {
                        Console.WriteLine("Warning: failed to translate key stroke");
                    }
                    else
                    {
                        _inputHandler.AsTextAndHotKeysHandler()(textOrHotKey);
                    }
                }
            };
        }

        public void SetInputHandler(InputMode<Unit> handler)
        {
            _inputHandler = handler;
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

        public void SubscribeToPaint(FSharpFunc<Action<DrawingObject>, Unit> paint)
        {
            Paint += (sender, eventArgs) => paint.Invoke(new WinFormsArtist(eventArgs.Graphics, _font).Draw);
        }

        public void TriggerDraw(PixelGrid.Block block)
        {
            Invalidate(block.AsWinFormsRectangle());
        }
    }
}
