using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;

namespace Void
{
    public class MainForm : Form
    {
		private readonly Font _font = Fonts.Monospace(9);
        private readonly int _lineHeight;
        private readonly int _lineWidth;

        public MainForm()
        {
            Title = "Void - A text editor in the spirit of Vim";
			BackgroundColor = Color.FromArgb(0, 0, 0);

            var verticalPadding = Platform.IsGtk ? 0 : 3;
            var horizontalPadding = Platform.IsGtk ? 0.0 : 2.57;
            _lineHeight = Convert.ToInt32(Math.Ceiling(_font.LineHeight))+verticalPadding;
            const int NumberOfColumns = 80;
            _lineWidth = Convert.ToInt32(Math.Ceiling((_font.XHeight+horizontalPadding)*NumberOfColumns));
            const int NumberOfRows = 25;
            var height = _lineHeight * NumberOfRows + verticalPadding;
			var size = new Size(_lineWidth, height);
			ClientSize = size;
			Content = PopulateContent(size, NumberOfRows);
        }

		private Control PopulateContent(Size size, int numberOfRows)
		{
			var content = new TableLayout(size);
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
            return new Label {
                Text = text,
                TextColor = Color.FromArgb(255, 255, 255),
                Font = _font,
                Height = _lineHeight,
                Width = _lineWidth
            };
        }

		private Label MakeLabel(int counter)
		{
            return MakeLabel("Line #" + counter);
		}
    }
}
