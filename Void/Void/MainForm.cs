using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using Void.ViewModel;

namespace Void
{
    public class MainForm : Form, MainView
    {
        private readonly MainController _controller;
		private Font _font;
        private int _lineHeight;

        public MainForm()
        {
            _controller = new MainController(this);
            _controller.init();
			Content = PopulateContent(25); // TODO do not hard-code
        }

        public FontMetrics GetFontMetrics()
        {
            var verticalPadding = Platform.IsGtk ? 0 : 3;
            var horizontalPadding = Platform.IsGtk ? 0.0 : 2.57;
            var height = _font.LineHeight + verticalPadding;
            var width = _font.XHeight + horizontalPadding;
            _lineHeight = Convert.ToInt32(height); // TODO DELETE
            return new FontMetrics(height, width);
        }

        public void SetBackgroundColor(RGBColor color)
        {
            BackgroundColor = Color.FromArgb(color.Red, color.Green, color.Blue);
        }

        public void SetFontBySize(byte size)
        {
            _font = Fonts.Monospace(size);
        }

        public void SetViewSize(SizeInPixels size)
        {
			ClientSize = new Size(size.Width, size.Height);
        }

        public void SetViewTitle(string title)
        {
            Title = title;
        }

		private Control PopulateContent(int numberOfRows)
		{
			var content = new TableLayout(ClientSize);
            var offset0 = Convert.ToInt32(Math.Floor(_font.LineHeight))*0;
            content.Add(MakeLabel("XWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWX"), new Point(0, offset0));
			foreach (var i in Enumerable.Range(1, numberOfRows-2))
			{
			    var offset = Convert.ToInt32(Math.Floor(_font.LineHeight))*i;
				content.Add(MakeLabel(i), new Point(0, offset));
			}
            var offset25 = Convert.ToInt32(Math.Floor(_font.LineHeight))*numberOfRows-1;
            content.Add(MakeLabel("XWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWX"), new Point(0, offset25));
			return content;
		}

        private Label MakeLabel(string text)
        {
            var fg = _controller.foregroundColor();
            return new Label {
                Text = text,
                TextColor = Color.FromArgb(fg.Red, fg.Green, fg.Blue),
                Font = _font,
                Height = _lineHeight,
                Width = ClientSize.Width
            };
        }

		private Label MakeLabel(int counter)
		{
            return MakeLabel("Line #" + counter);
		}
    }
}
