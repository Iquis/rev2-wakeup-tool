using System.Diagnostics;
using GGXrdWakeupDPUtil.Library.Memory;

namespace GGXrdWakeupDPUtil.Library.Replay.AsmInjection
{
    public class AsmInjectionTriggerFactory : ReplayTriggerFactory
    {
        private readonly Process _process;
        private readonly MemoryReader _memoryReader;

        public AsmInjectionTriggerFactory(Process process, MemoryReader memoryReader)
        {
            _process = process;
            _memoryReader = memoryReader;
        }

        public override ReplayTrigger GetReplayTrigger()
        {
            return new AsmInjectionReplayTrigger(this._process, this._memoryReader);
        }
    }
}
