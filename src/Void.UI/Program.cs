using System;
using System.Windows.Forms;
using Void.Core;
using Void.Util;
using Void.ViewModel;
using Message = Void.Core.Message;

namespace Void.UI
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var inputModeChanger = new WinFormsInputModeChanger();
            Bus bus = Init.buildVoid(inputModeChanger, Options.parse(args));
            var view = new MainForm(bus, inputModeChanger);
            bus.subscribe(FSharpFuncUtil.Create<CoreEvent, Message>(view.HandleEvent));
            bus.subscribe(FSharpFuncUtil.Create<VMEvent, Message>(view.HandleViewModelEvent));
            Init.launchVoid(bus);
            Application.Run(view);
        }
    }
}
