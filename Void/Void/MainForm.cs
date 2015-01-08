using Eto.Forms;
using Eto.Drawing;

namespace Void
{
    public class MainForm : Form
    {
        public MainForm()
        {
            Title = "My Eto Form";
            ClientSize = new Size(400, 350);

            // scrollable region as the main content
            Content = new Scrollable
            {
                // table with three rows
                Content = new TableLayout(
                    null,
                    // row with three columns
                    new TableRow(null, new Label { Text = "Hello World!" }, null),
                    null
                )
            };

            // create a few commands that can be used for the menu and toolbar
            var clickMe = new Command { MenuText = "Click Me!", ToolBarText = "Click Me!" };
            clickMe.Executed += (sender, e) => MessageBox.Show(this, "I was clicked!");

            var quitCommand = new Command { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
            quitCommand.Executed += (sender, e) => Application.Instance.Quit();

            var aboutCommand = new Command { MenuText = "About..." };
            aboutCommand.Executed += (sender, e) => MessageBox.Show(this, "About my app...");

            Menu = new MenuBar();
        }
    }
}
