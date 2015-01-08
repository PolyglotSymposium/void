using System;
using Eto.Forms;

namespace Void.Mac
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Application(Eto.Platforms.Mac).Run(new MainForm());
        }
    }
}

