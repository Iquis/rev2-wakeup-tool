using System;
using System.Collections.Generic;

namespace GGXrdWakeupDPUtil.Library.Replay.AsmInjection
{
    public class MemoryAllocator : IDisposable
    {
        private readonly List<MemoryAllocation> _memoryAllocations = new List<MemoryAllocation>();

        public IntPtr AddAllocation(MemoryAllocation memoryAllocation)
        {
            this._memoryAllocations.Add(memoryAllocation);

            return memoryAllocation.Allocate();
        }

        public IntPtr AddAllocation(IntPtr handle, int size)
        {
            MemoryAllocation memoryAllocation = new MemoryAllocation(handle, size);

            return this.AddAllocation(memoryAllocation);
        }

        public void Dispose()
        {
            foreach (var memoryAllocation in _memoryAllocations)
            {
                memoryAllocation.Dispose();
            }
        }
    }
}
