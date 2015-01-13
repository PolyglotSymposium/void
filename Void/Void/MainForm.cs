using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;

namespace Void
{
    public class MainForm : Form
    {
        private readonly ViewModel.VoidController _controller = new ViewModel.VoidController();
		private readonly Font _font = Fonts.Monospace(9);
        private readonly int _lineHeight;
        private readonly int _lineWidth;

        public MainForm()
        {
            Title = _controller.titlebarText();
            var bg = _controller.backgroundColor();
			BackgroundColor = Color.FromArgb(bg.Red, bg.Green, bg.Blue);

            var dimensions = _controller.startupDimensions();
            var verticalPadding = Platform.IsGtk ? 0 : 3;
            var horizontalPadding = Platform.IsGtk ? 0.0 : 2.57;
            _lineHeight = Convert.ToInt32(Math.Ceiling(_font.LineHeight))+verticalPadding;
            _lineWidth = Convert.ToInt32(Math.Ceiling((_font.XHeight+horizontalPadding)*dimensions.Columns));
            var height = _lineHeight * dimensions.Rows + verticalPadding;
			var size = new Size(_lineWidth, height);
			ClientSize = size;
			Content = PopulateContent(size, dimensions.Rows);
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
            var fg = _controller.foregroundColor();
            return new Label {
                Text = text,
                TextColor = Color.FromArgb(fg.Red, fg.Green, fg.Blue),
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
