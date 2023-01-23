using System.Threading;

namespace GGXrdWakeupDPUtil.Library.Replay.Keyboard
{
    public class KeyboardReplayTrigger : ReplayTrigger
    {
        private readonly Library.Keyboard.DirectXKeyStrokes _stroke;


        public KeyboardReplayTrigger(Library.Keyboard.DirectXKeyStrokes stroke)
        {
            this._stroke = stroke;
        }

        public override void TriggerReplay()
        {
            LogManager.Instance.WriteLine("Trigger Replay by Keyboard stroke");

            Library.Keyboard keyboard = new Library.Keyboard();
            
            keyboard.SendKey(_stroke, false, Library.Keyboard.InputType.Keyboard);
            Thread.Sleep(150);
            keyboard.SendKey(_stroke, true, Library.Keyboard.InputType.Keyboard);
        }
    }
}
