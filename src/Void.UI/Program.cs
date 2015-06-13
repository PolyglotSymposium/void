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
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var inputModeChanger = new WinFormsInputModeChanger();
            var messagingSystem = Init.buildVoid(inputModeChanger);
            var view = new MainForm(messagingSystem.Bus, inputModeChanger);
            messagingSystem.EventChannel.addHandler(FSharpFuncUtil.Create<Event, Message>(view.HandleEvent));
            messagingSystem.VMEventChannel.addHandler(FSharpFuncUtil.Create<VMEvent, Message>(view.HandleViewModelEvent));
            Init.launchVoid(messagingSystem);
            Application.Run(view);
        }
    }
}
