using System;

namespace GGXrdWakeupDPUtil.Library.Replay
{
    public abstract class ReplayTrigger: IDisposable
    {
       public abstract void TriggerReplay();

       public virtual void InitTrigger()
       {
           LogManager.Instance.WriteLine($"Initialization {this.GetType().Name}");
        }

       public virtual void Dispose()
       {

       }
    }
}
