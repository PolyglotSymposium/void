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

        public MainForm()
        {
            const int LinePadding = 3;
            _lineHeight = Convert.ToInt32(Math.Ceiling(_font.LineHeight))+LinePadding;
            Title = "Void - A text editor in the spirit of Vim";
            const int NumberOfRows = 25;
            var height = _lineHeight * NumberOfRows + LinePadding;
			var size = new Size(480, height);
			BackgroundColor = Color.FromArgb(0, 0, 0);
			Content = PopulateContent(size, NumberOfRows);
			ClientSize = size;
        }

		private Control PopulateContent(Size size, int numberOfRows)
		{
			var content = new TableLayout(size);
			foreach (var i in Enumerable.Range(0, numberOfRows))
			{
			    var offset = Convert.ToInt32(Math.Floor(_font.LineHeight))*i;
				content.Add(MakeLabel(i), new Point(0, offset));
			}
			return content;
		}

		private Label MakeLabel(int counter)
		{
			return new Label {
				Text = "Line #" + counter,
				TextColor = Color.FromArgb(255, 255, 255),
				Font = _font,
                Height = _lineHeight
			};
		}
    }
}
