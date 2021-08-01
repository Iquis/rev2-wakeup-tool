using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace GGXrdWakeupDPUtil.Library.Memory
{
    public class MemoryReader
    {
        private readonly Process _process;


        #region DLL Imports
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            int dwSize,
            ref int lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory
            (IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            int dwSize,
            ref int lpNumberOfBytesRead);
        #endregion

        public MemoryReader(Process process)
        {
            this._process = process;
        }
        
        public bool Write(IntPtr address, IEnumerable<byte> bytes)
        {
            IntPtr handle = this._process.Handle;
            int lpNumberOfBytesRead = 0;
            return WriteProcessMemory(handle, address, bytes.ToArray(), bytes.Count(), ref lpNumberOfBytesRead);
        }


        public bool Write(IntPtr address, IEnumerable<ushort> shorts)
        {
            List<byte> bytes = new List<byte>();

            foreach (ushort @ushort in shorts)
            {
                bytes.Add((byte)(@ushort & 0xFF));
                bytes.Add((byte)((@ushort >> 8) & 0xFF));
            }

            return Write(address, bytes.ToArray());

        }

        public byte[] ReadBytes(IntPtr address, int length)
        {
            IntPtr handle = this._process.Handle;
            byte[] bytes = new byte[length];
            int lpNumberOfBytesRead = 0;
            ReadProcessMemory(handle, address, bytes, bytes.Length, ref lpNumberOfBytesRead);

            return bytes;
        }

        public T Read<T>(IntPtr address)
        {
            Type outputType = typeof(T).IsEnum ? Enum.GetUnderlyingType(typeof(T)) : typeof(T);

            int length = Marshal.SizeOf(outputType);

            var value = this.ReadBytes(address, length);


            T result;

            GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned);

            try
            {
                result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), outputType);
            }
            finally
            {
                handle.Free();
            }

            return result;
        }

        public T ReadWithOffsets<T>(IntPtr baseAddress, params int[] offsets)
        {
            IntPtr newAddress = IntPtr.Add(this._process.MainModule.BaseAddress, (int)baseAddress);
            IntPtr value = this.Read<IntPtr>(newAddress);
            T result = UnmanagedConvert<T>(value);

            for (int i = 0; i < offsets.Length; i++)
            {
                newAddress = IntPtr.Add(value, offsets[i]);

                if (i + 1 == offsets.Length)
                {
                    result = this.Read<T>(newAddress);
                }
                else
                {
                    value = this.Read<IntPtr>(newAddress);
                }
            }

            return result;
        }
        private T UnmanagedConvert<T>(object value)
        {
            Type outputType = typeof(T).IsEnum ? Enum.GetUnderlyingType(typeof(T)) : typeof(T);

            T result;

            GCHandle handle = GCHandle.Alloc(value, GCHandleType.Pinned);

            try
            {
                result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), outputType);
            }
            finally
            {
                handle.Free();
            }

            return result;
        }

        public string ReadString(IntPtr address, int length)
        {
            var value = this.ReadBytes(address, length);
            var result = Encoding.Default.GetString(value);
            return result?.Replace("\0", "");
        }

        public string ReadStringWithOffsets(IntPtr baseAddress, int length, params int[] offsets)
        {
            IntPtr newAddress = IntPtr.Add(this._process.MainModule.BaseAddress, (int)baseAddress);
            IntPtr value = this.Read<IntPtr>(newAddress);
            string result = string.Empty;

            for (int i = 0; i < offsets.Length; i++)
            {
                newAddress = IntPtr.Add(value, offsets[i]);

                if (i + 1 == offsets.Length)
                {
                    result = this.ReadString(newAddress, length);
                }
                else
                {
                    value = this.Read<IntPtr>(newAddress);
                }
            }

            return result;
        }
    }
}
