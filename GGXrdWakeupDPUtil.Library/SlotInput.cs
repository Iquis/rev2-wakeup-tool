using System.Collections.Generic;

namespace GGXrdWakeupDPUtil.Library
{
    public class SlotInput
    {
        public SlotInput(string input, IEnumerable<ushort> inputShorts, int wakeupFrameIndex)
        {
            Input = input;
            InputList = inputShorts;
            WakeupFrameIndex = wakeupFrameIndex;
        }

        public string Input { get; set; }
        public IEnumerable<ushort> InputList { get; set; }
        public int WakeupFrameIndex { get; set; }

    }
}
