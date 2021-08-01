namespace GGXrdWakeupDPUtil.Library.Replay.Keyboard
{
    public class KeyboardTriggerFactory : ReplayTriggerFactory
    {
        private readonly Library.Keyboard.DirectXKeyStrokes _stroke;

        public KeyboardTriggerFactory(Library.Keyboard.DirectXKeyStrokes stroke)
        {
            this._stroke = stroke;
        }

        public override ReplayTrigger GetReplayTrigger()
        {
            return new KeyboardReplayTrigger(this._stroke);
        }
    }
}
