using System.Linq;
using Eto.Forms;
using Eto.Drawing;

namespace Void
{
    public class MainForm : Form
    {
		private Font _font = Fonts.Monospace(9);

        public MainForm()
        {
            Title = "Void - A text editor in the spirit of Vim";
			var size = new Size(480, System.Convert.ToInt32(System.Math.Ceiling(_font.LineHeight*25)));
			ClientSize = size;
			BackgroundColor = Color.FromArgb(0, 0, 0);
			Content = PopulateContent(size);
        }

		private Control PopulateContent(Size size)
		{
			var content = new TableLayout(size);
			foreach (var i in Enumerable.Range(0, 25))
			{
				content.Add(MakeLabel(i), new Point(0, 17*i));
			}
			return content;
		}


		private Label MakeLabel(int counter)
		{
			return new Label {
				Text = "Line #" + counter,
				TextColor = Color.FromArgb(255, 255, 255),
				Font = _font
			};
		}
    }
}
