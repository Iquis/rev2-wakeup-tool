﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using GGXrdReversalTool.Library.Characters;
using GGXrdReversalTool.Library.Memory.Pointer;
using GGXrdReversalTool.Library.Models.Inputs;

namespace GGXrdReversalTool.Library.Memory.Implementations;

//TODO Lint
public class MemoryReader : IMemoryReader
{
    public MemoryReader(Process process)
    {
        Process = process;
        _pointerCollection = new MemoryPointerCollection(process, this);
    }

    private const int RecordingSlotSize = 4808;

    private readonly MemoryPointerCollection _pointerCollection;

    public Process Process { get; }

    public string ReadAnimationString(int player)
    {
        const int length = 32;

        return player switch
        {
            0 => ReadString(_pointerCollection.P1AnimStringPtr, length),
            1 => ReadString(_pointerCollection.P2AnimStringPtr, length),
            _ => string.Empty
        };
    }

    public int FrameCount()
    {
        return Read<int>(_pointerCollection.FrameCountPtr);
    }

    public Character GetCurrentDummy()
    {
        var index = GetPlayerSide() switch
        {
            0 => Read<byte>(_pointerCollection.P2CharIDPtr),
            1 => Read<byte>(_pointerCollection.P1CharIDPtr),
            _ => 0
        };
        var result = Character.Characters[index];

        return result;
    }

    public bool SetDummyPlayback(int slotNumber, int inputIndex, int startingSide)
    {
        if (slotNumber is < 1 or > 3)
        {
            throw new ArgumentException("Invalid Slot number", nameof(slotNumber));
        }
        var result = true;
        result = result && Write(_pointerCollection.DummyRecInputsSlotPtr, slotNumber - 1);
        result = result && Write(_pointerCollection.DummyRecInputsIndexPtr, inputIndex);
        result = result && Write(_pointerCollection.DummyRecInputsSidePtr, startingSide);
        result = result && Write(_pointerCollection.DummyModePtr, 3);
        return result;
    }

    public bool WriteInputInSlot(int slotNumber, SlotInput slotInput)
    {
        if (slotNumber is < 1 or > 3)
        {
            throw new ArgumentException("Invalid Slot number", nameof(slotNumber));
        }

        var baseAddress = GetAddressWithOffsets(_pointerCollection.RecordingSlotPtr);
        var slotAddress = IntPtr.Add(baseAddress, RecordingSlotSize * (slotNumber - 1));

        return Write(slotAddress, slotInput.Header.Concat(slotInput.Content));
    }

    public SlotInput ReadInputFromSlot(int slotNumber)
    {
        if (slotNumber is < 1 or > 3)
        {
            throw new ArgumentException("Invalid Slot number", nameof(slotNumber));
        }

        var baseAddress = GetAddressWithOffsets(_pointerCollection.RecordingSlotPtr);
        var slotAddress = IntPtr.Add(baseAddress, RecordingSlotSize * (slotNumber - 1));

        var readBytes = ReadBytes(slotAddress, RecordingSlotSize);


        var inputLength = byte.MaxValue * readBytes[5] + readBytes[4];

        var headerLength = 4;

        var length = 2 * (inputLength + headerLength);


        var result = new byte[length];
        Array.Copy(readBytes, result, 2 * (inputLength + headerLength));

        return new SlotInput(result);

    }

    public int GetComboCount(int player)
    {
        return player switch
        {
            0 => Read<int>(_pointerCollection.P1ComboCountPtr),
            1 => Read<int>(_pointerCollection.P2ComboCountPtr),
            _ => throw new ArgumentException($"Player index is invalid : {player}")
        };
    }

    public int GetBlockstun(int player)
    {
        return player switch
        {
            0 => Read<int>(_pointerCollection.P1BlockStunPtr),
            1 => Read<int>(_pointerCollection.P2BlockStunPtr),
            _ => throw new ArgumentException($"Player index is invalid : {player}")
        };
    }

    public int GetFacing(int player)
    {
        return player switch
        {
            0 => Read<int>(_pointerCollection.P1FacingPtr),
            1 => Read<int>(_pointerCollection.P2FacingPtr),
            _ => throw new ArgumentException($"Player index is invalid : {player}")
        };
    }

    public int GetPlayerSide()
    {
        return Read<byte>(_pointerCollection.PlayerSidePtr) != 0 ? 1 : 0;
    }

    public bool IsTrainingMode()
    {
        return Read<byte>(_pointerCollection.GameModePtr) == 6;
    }

    public bool IsWorldInTick()
    {
        return Read<int>(_pointerCollection.WorldInTickPtr) != 0;
    }

    public uint GetEngineTickCount()
    {
        // Actually a 64-bit counter but it takes 28 months of continuous runtime to overflow into the high dword
        return Read<uint>(_pointerCollection.EngineTickCountPtr);
    }


    #region DLL Imports

    [DllImport("kernel32.dll")]
    private static extern bool ReadProcessMemory(
        IntPtr hProcess,
        IntPtr lpBaseAddress,
        byte[] lpBuffer,
        int dwSize,
        ref int lpNumberOfBytesRead);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteProcessMemory(
        IntPtr hProcess,
        IntPtr lpBaseAddress,
        byte[] lpBuffer,
        int dwSize,
        ref int lpNumberOfBytesRead);



    #endregion

    private IntPtr GetAddressWithOffsets(MemoryPointer memoryPointer)
    {
        var address = memoryPointer.Pointer;
        foreach (var offset in memoryPointer.Offsets)
        {
            address = ReadPtr(address) + offset;
        }

        return address;
    }

    private string ReadString(MemoryPointer memoryPointer, int length)
    {
        var value = ReadBytes(GetAddressWithOffsets(memoryPointer), length);
        var result = Encoding.Default.GetString(value);
        return result.Replace("\0", "");
    }

    private T Read<T>(IntPtr address)
    {
        var outputType = typeof(T).IsEnum ? Enum.GetUnderlyingType(typeof(T)) : typeof(T);

        var length = Marshal.SizeOf(outputType);

        var value = this.ReadBytes(address, length);


        T result;

        var handle = GCHandle.Alloc(value, GCHandleType.Pinned);

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

    private IntPtr ReadPtr(IntPtr address)
    {
        return new IntPtr(Read<int>(address));
    }

    private byte[] ReadBytes(IntPtr address, int length)
    {
        var handle = Process.Handle;
        var bytes = new byte[length];
        var lpNumberOfBytesRead = 0;
        ReadProcessMemory(handle, address, bytes, bytes.Length, ref lpNumberOfBytesRead);

        return bytes;
    }

    private T Read<T>(MemoryPointer memoryPointer)
    {
        return Read<T>(GetAddressWithOffsets(memoryPointer));
    }

    private bool Write(IntPtr address, IEnumerable<ushort> shorts)
    {
        var bytes = new List<byte>();

        foreach (var @ushort in shorts)
        {
            bytes.Add((byte)(@ushort & 0xFF));
            bytes.Add((byte)((@ushort >> 8) & 0xFF));
        }

        return Write(address, bytes);

    }

    private bool Write(IntPtr address, IEnumerable<byte> bytes)
    {
        var byteArray = bytes.ToArray();
        var handle = Process.Handle;
        var lpNumberOfBytesRead = 0;
        return WriteProcessMemory(handle, address, byteArray, byteArray.Length, ref lpNumberOfBytesRead);
    }

    private bool Write(IntPtr address, int val) => Write(address, BitConverter.GetBytes(val));
    private bool Write(MemoryPointer memoryPointer, int val) => Write(GetAddressWithOffsets(memoryPointer), val);

    private class MemoryPointerCollection
    {
        public MemoryPointer P1CharIDPtr { get; private set; } = null!;
        public MemoryPointer P2CharIDPtr { get; private set; } = null!;
        public MemoryPointer P1AnimStringPtr { get; private set; } = null!;
        public MemoryPointer P2AnimStringPtr { get; private set; } = null!;
        public MemoryPointer FrameCountPtr { get; private set; } = null!;
        public MemoryPointer RecordingSlotPtr { get; private set; } = null!;
        public MemoryPointer P1ComboCountPtr { get; private set; } = null!;
        public MemoryPointer P2ComboCountPtr { get; private set; } = null!;
        public MemoryPointer P1BlockStunPtr { get; private set; } = null!;
        public MemoryPointer P2BlockStunPtr { get; private set; } = null!;
        public MemoryPointer P1FacingPtr { get; private set; } = null!;
        public MemoryPointer P2FacingPtr { get; private set; } = null!;
        public MemoryPointer PlayerSidePtr { get; private set; } = null!;
        public MemoryPointer GameModePtr { get; private set; } = null!;
        public MemoryPointer DummyModePtr { get; private set; } = null!;
        public MemoryPointer DummyRecInputsSlotPtr { get; private set; } = null!;
        public MemoryPointer DummyRecInputsIndexPtr { get; private set; } = null!;
        public MemoryPointer DummyRecInputsSidePtr { get; private set; } = null!;
        public MemoryPointer WorldInTickPtr { get; private set; } = null!;
        public MemoryPointer EngineTickCountPtr { get; private set; } = null!;

        private readonly Process _process;
        private readonly MemoryReader _memoryReader;

        public MemoryPointerCollection(Process process, MemoryReader memoryReader)
        {
            _process = process;
            _memoryReader = memoryReader;
            BuildCollection();
        }

        private void BuildCollection()
        {
            var textAddr = _process.MainModule!.BaseAddress + 0x1000;

            if (VirtualQueryEx(_process.Handle, textAddr, out var memoryBasicInformation64,
                    (uint)Marshal.SizeOf(typeof(MemoryBasicInformation64))) == 0)
            {
                throw new Exception("Failed to retrieve information about .text allocation");
            }

            var text = _memoryReader.ReadBytes(textAddr, (int)memoryBasicInformation64.RegionSize);

            const string playerPattern = "i4Ew5iIAi4B8AwAAM9s7w3QPi4DIAQAAg+ABiUQkKOs=";
            var matchPtrAddr = _memoryReader.Read<int>(textAddr - 4 + FindPatternOffset(text, playerPattern));

            const int playerOffset = 0x169814;
            const int playerSize = 0x2d198;

            P1CharIDPtr = new MemoryPointer(matchPtrAddr, playerOffset + 0x44);
            P2CharIDPtr = new MemoryPointer(matchPtrAddr, playerOffset + playerSize + 0x44);
            P1AnimStringPtr = new MemoryPointer(matchPtrAddr, playerOffset + 0x2444);
            P2AnimStringPtr = new MemoryPointer(matchPtrAddr, playerOffset + playerSize + 0x2444);
            P2ComboCountPtr = new MemoryPointer(matchPtrAddr, playerOffset + 0x9f28);
            P1ComboCountPtr = new MemoryPointer(matchPtrAddr, playerOffset + playerSize + 0x9f28);
            P1BlockStunPtr = new MemoryPointer(matchPtrAddr, playerOffset + 0x4d54);
            P2BlockStunPtr = new MemoryPointer(matchPtrAddr, playerOffset + playerSize + 0x4d54);
            P1FacingPtr = new MemoryPointer(matchPtrAddr, playerOffset + 0x4d38);
            P2FacingPtr = new MemoryPointer(matchPtrAddr, playerOffset + playerSize + 0x4d38);


            const string recordingSlotPattern = "i1QkBGnSyBIAAIHC";
            RecordingSlotPtr =
                new MemoryPointer(
                    _memoryReader.Read<int>(textAddr + 12 + FindPatternOffset(text, recordingSlotPattern)));

            const string frameCountPattern = "0o1U0AhBiYioAAAAxwIEAAAAiV8Mx0cUAwAAAF9eiR0=";
            FrameCountPtr =
                new MemoryPointer(_memoryReader.Read<int>(textAddr + 32 + FindPatternOffset(text, frameCountPattern)));

            // Global UREDGameCommon instance, containing high level game state and resources
            // This reference is in GetHitoriyouMainSide (name known because of debug printf)
            const string getHitoriyouMainSidePattern = "M8A4QUQPlcDDzA==";
            var gameDataPtr = _memoryReader.Read<int>(textAddr - 4 + FindPatternOffset(text, getHitoriyouMainSidePattern));
            PlayerSidePtr = new MemoryPointer(gameDataPtr, 0x44); // Internally "MainPlayer"
            GameModePtr = new MemoryPointer(gameDataPtr, 0x45); // Internally "GameModeID"

            // Global structure mostly (entirely?) containing data for training and other single player modes
            const string trainingStructPattern = "mbkDAAAA9/mNev+LRkBVULk=";
            var trainingStructAddr = _memoryReader.Read<int>(textAddr + 17 + FindPatternOffset(text, trainingStructPattern));
            DummyModePtr = new MemoryPointer(trainingStructAddr + 0);
            DummyRecInputsSlotPtr = new MemoryPointer(trainingStructAddr + 4);
            DummyRecInputsIndexPtr = new MemoryPointer(trainingStructAddr + 0x19cc);
            DummyRecInputsSidePtr = new MemoryPointer(trainingStructAddr + 0x19d0);

            // Global UWorld instance
            const string theWorldPattern = "VovxV4t+KDt4UA==";
            var theWorldPtrAddr = _memoryReader.Read<int>(textAddr - 4 + FindPatternOffset(text, theWorldPattern));
            WorldInTickPtr = new MemoryPointer(theWorldPtrAddr, 0x14c);

            // Global 64 bit tick counter for the main loop incremented shortly after ticking everything
            const string engineTickCountPattern = "dQWD+AV2FPIPEEcQ";
            EngineTickCountPtr = new MemoryPointer(_memoryReader.Read<int>(textAddr - 4 + FindPatternOffset(text, engineTickCountPattern)));
        }

        private int FindPatternOffset(in byte[] haystack, in byte[] needle)
        {
            // Boyer-Moore-Horspool substring search
            var needleLen = needle.Length;
            var step = Enumerable.Repeat(needleLen, 256).ToArray();
            for (var i = 0; i < needleLen - 1; i++)
            {
                step[needle[i]] = needleLen - 1 - i;
            }

            var end = haystack.Length - needleLen;
            for (var p = 0; p <= end; p += step[haystack[p + needleLen - 1]])
            {
                int j;
                for (j = needleLen; --j >= 0;)
                {
                    if (needle[j] != haystack[p + j])
                    {
                        break;
                    }
                }

                if (j < 0)
                {
                    return p;
                }
            }

            throw new Exception("Could not find offset in process");
        }

        private int FindPatternOffset(in byte[] haystack, in string needle) =>
            FindPatternOffset(haystack, Convert.FromBase64String(needle));


        #region DLL Imports

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int VirtualQueryEx(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            out MemoryBasicInformation64 lpBuffer,
            uint dwSize);

        [StructLayout(LayoutKind.Sequential)]
        private struct MemoryBasicInformation64
        {
            private readonly ulong BaseAddress;
            private readonly ulong AllocationBase;
            private readonly uint AllocationProtect;
            private readonly uint Alignment1;
            public readonly ulong RegionSize;
            private readonly uint State;
            private readonly uint Protect;
            private readonly uint Type;
            private readonly uint Alignment2;
        }

        #endregion

    }
}
