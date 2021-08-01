using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGXrdWakeupDPUtil.Library.Replay
{
    public abstract class ReplayTriggerFactory
    {
        public abstract ReplayTrigger GetReplayTrigger();
    }
}
