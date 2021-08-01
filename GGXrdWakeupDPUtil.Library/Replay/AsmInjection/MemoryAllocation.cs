using System;
using System.Runtime.InteropServices;

namespace GGXrdWakeupDPUtil.Library.Replay.AsmInjection
{
    public class MemoryAllocation : IDisposable
    {
        #region DLL Imports

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            int dwSize,
            int flAllocationType,
            int flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool VirtualFreeEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            int dwSize,
            int dwFreeType);

        #endregion

        public MemoryAllocation(IntPtr handle, int size, MemoryAllocationFlags memoryAllocationFlag = MemoryAllocationFlags.Commit, MemoryProtectionFlags memoryProtectionFlag = MemoryProtectionFlags.ExecuteReadWrite)
        {
            this.Handle = handle;
            this.Size = size;
            this.MemoryAllocationFlag = memoryAllocationFlag;
            this.MemoryProtectionFlag = memoryProtectionFlag;

        }

        public IntPtr Handle { get; }

        public int Size { get; }

        public MemoryAllocationFlags MemoryAllocationFlag { get; }

        public MemoryProtectionFlags MemoryProtectionFlag { get; }

        public IntPtr Allocate()
        {
            return VirtualAllocEx(this.Handle, IntPtr.Zero, this.Size, (int)this.MemoryAllocationFlag, (int)this.MemoryProtectionFlag);
        }

        private void Release()
        {
            VirtualFreeEx(this.Handle, IntPtr.Zero, this.Size, (int)MemoryReleaseFlags.Release);
        }

        public void Dispose()
        {
            this.Release();
        }
    }
}
