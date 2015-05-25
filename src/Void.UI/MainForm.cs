using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Void.Core;
using Void.Util;
using Void.ViewModel;
using Microsoft.FSharp.Core;
using Message = Void.Core.Message;

namespace Void.UI
{
    public partial class MainForm : Form, InputModeChanger
    {
        private Font _font = new Font(FontFamily.GenericMonospace, 9);
        private InputMode<Unit> _inputHandler;
        private CellMetrics _cellMetrics;
        private IEnumerable<DrawingObject> _drawings;

        public MainForm()
        {
            InitializeComponent();
            SubscribeToPaint();
            WireUpInputEvents();
        }

        public Message HandleEvent(Event eventMsg)
        {
            if (eventMsg.IsLastWindowClosed)
            {
                Close();
            }
            return null;
        }

        public Message HandleViewModelEvent(VMEvent eventMsg)
        {
            if (eventMsg.IsViewPortionRendered)
            {
                _drawings = ((VMEvent.ViewPortionRendered)eventMsg).Item2;
                TriggerDraw(((VMEvent.ViewPortionRendered)eventMsg).Item1);
            }
            if (eventMsg.IsViewModelInitialized)
            {
                var viewModel = ((VMEvent.ViewModelInitialized)eventMsg).Item;
                SetBackgroundColor(viewModel.BackgroundColor);
                SetFontBySize(viewModel.FontSize);
                SetViewSize(viewModel.Size);
                SetViewTitle(viewModel.Title);
            }
            return null;
        }

        private void WireUpInputEvents()
        {
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

        public void SubscribeToPaint()
        {
            Paint += (sender, eventArgs) =>
            {
                var artist = new WinFormsArtist(eventArgs.Graphics, _font, _cellMetrics);
                foreach (var drawing in _drawings)
                {
                    artist.Draw(drawing);
                }
                _drawings = Enumerable.Empty<DrawingObject>();
            };
        }

        public void SetInputHandler(InputMode<Unit> handler)
        {
            _inputHandler = handler;
        }

        private int MeasureFontWidth()
        {
            // TODO this isn't working 100% well
            // Ironically, it seems to work better on Mono than .NET
            return Convert.ToInt32(Math.Ceiling(CreateGraphics().MeasureString(new string('X', 80), _font).Width / 80));
        }

        private void SetBackgroundColor(RGBColor color)
        {
            BackColor = color.AsWinFormsColor();
        }

        private void SetFontBySize(int size)
        {
            _font = new Font(FontFamily.GenericMonospace, size);
            _cellMetrics = new CellMetrics(_font.Height, MeasureFontWidth());
        }

        private void SetViewSize(CellGrid.Dimensions size)
        {
            ClientSize = size.AsWinFormsSize(_cellMetrics);
        }

        private void SetViewTitle(string title)
        {
            Text = title;
        }

        public void TriggerDraw(PointGrid.Block block)
        {
            Invalidate(block.AsWinFormsRectangle(_cellMetrics));
        }
    }
}
