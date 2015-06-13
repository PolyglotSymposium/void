using Microsoft.FSharp.Core;
using Void.Core;

namespace Void.UI
{
    public class WinFormsInputModeChanger : InputModeChanger
    {
        public InputMode<Unit> InputHandler { get; private set; }

        public void SetInputHandler(InputMode<Unit> handler)
        {
            InputHandler = handler;
        }
    }
}