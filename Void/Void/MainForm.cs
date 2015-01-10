using Eto.Forms;
using Eto.Drawing;

namespace Void
{
    public class MainForm : Form
    {
        public MainForm()
        {
            Title = "Void - A text editor in the spirit of Vim";
            ClientSize = new Size(480, 320);

            // scrollable region as the main content
            Content = new Scrollable
            {
                Content = new TableLayout(
                    new TableRow(new Label
                    {
                        Text = "Hello World!",
                        TextColor = Color.FromArgb(255, 255, 255),
                        Font = Fonts.Monospace(9)
                    })
                ),
                BackgroundColor = Color.FromArgb(0, 0, 0)
            };

            Menu = new MenuBar();
        }
    }
}
