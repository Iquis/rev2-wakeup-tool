using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using GGXrdReversalTool.Library.Characters;
using GGXrdReversalTool.Library.Memory.Pointer;
using GGXrdReversalTool.Library.Models.Inputs;

namespace GGXrdReversalTool.Library.Memory.Implementations;

//TODO Lint
public class MemoryReader : IMemoryReader
{
    private readonly Process _process;

    public MemoryReader(Process process)
    {
        _process = process;
    }

    private readonly MemoryPointer _p1AnimStringPtr = new("P1AnimStringPtr");
    private readonly MemoryPointer _p2AnimStringPtr = new("P2AnimStringPtr");
    private readonly MemoryPointer _frameCountPtr = new("FrameCountPtr");
    private readonly MemoryPointer _dummyIdPtr = new("DummyIdPtr");
    private readonly MemoryPointer _recordingSlotPtr = new("RecordingSlotPtr");
    private readonly MemoryPointer _p1ComboCountPtr = new("P1ComboCountPtr");
    private readonly MemoryPointer _p2ComboCountPtr = new("P2ComboCountPtr");
    private readonly MemoryPointer _p1ReplayKeyPtr = new("P1ReplayKeyPtr");
    private readonly MemoryPointer _p2ReplayKeyPtr = new("P2ReplayKeyPtr");
    private const int RecordingSlotSize = 4808;


    public Process Process => _process;

    public string ReadAnimationString(int player)
    {
        const int length = 32;

        switch (player)
        {
            case 1:
                return ReadString(_p1AnimStringPtr, length);
            case 2:
                return ReadString(_p2AnimStringPtr, length);
            default:
                return string.Empty;
        }
    }

    public int FrameCount()
    {
        return Read<int>(_frameCountPtr);
    }

    public Character GetCurrentDummy()
    {
        var index = Read<int>(_dummyIdPtr);
        var result = Character.Characters[index];

        return result;
    }


    public bool WriteInputInSlot(int slotNumber, SlotInput slotInput)
    {
        if (slotNumber is < 1 or > 3)
        {
            throw new ArgumentException("Invalid Slot number", nameof(slotNumber));
        }
        var baseAddress = GetAddressWithOffsets(_recordingSlotPtr.Pointer, _recordingSlotPtr.Offsets.ToArray());
        var slotAddress = IntPtr.Add(baseAddress, RecordingSlotSize * (slotNumber - 1));

        return Write(slotAddress, slotInput.Content);
    }

    public int GetComboCount(int player)
    {
        return player switch
        {
            1 => Read<int>(_p1ComboCountPtr),
            2 => Read<int>(_p2ComboCountPtr),
            _ => throw new ArgumentException($"Player index is invalid : {player}")
        };
    }

    public int GetReplayKeyCode(int player)
    {
        return player switch
        {
            1 => Read<int>(_p1ReplayKeyPtr),
            2 => Read<int>(_p2ReplayKeyPtr),
            _ => throw new ArgumentException($"Player index is invalid : {player}")
        };
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
    private static extern bool WriteProcessMemory
    (IntPtr hProcess,
        IntPtr lpBaseAddress,
        byte[] lpBuffer,
        int dwSize,
        ref int lpNumberOfBytesRead);

    #endregion
    
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

    private IntPtr GetAddressWithOffsets(IntPtr address, params int[] offsets)
    {
            
        IntPtr newAddress = IntPtr.Add(this._process.MainModule.BaseAddress, (int)address);

        foreach (var offset in offsets)
        {
            IntPtr value = new IntPtr(this.Read<int>(newAddress));
                
            newAddress = IntPtr.Add(value, offset);
        }

        return newAddress;
            
    }
    private string ReadString(MemoryPointer memoryPointer, int length)
    {
        return memoryPointer.Offsets.Any()
            ? this.ReadStringWithOffsets(memoryPointer.Pointer, length, memoryPointer.Offsets.ToArray())
            : this.ReadString(memoryPointer.Pointer, length);
    }

    private string ReadStringWithOffsets(IntPtr baseAddress, int length, params int[] offsets)
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

    private T Read<T>(IntPtr address)
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
    private T ReadWithOffsets<T>(IntPtr baseAddress, params int[] offsets)
    {
        IntPtr newAddress = IntPtr.Add(this._process.MainModule.BaseAddress, (int)baseAddress);

        foreach (var offset in offsets)
        {
            IntPtr value = new IntPtr(Read<int>(newAddress));
                
            newAddress = IntPtr.Add(value, offset);
        }

        return Read<T>(newAddress);
    }

    private string ReadString(IntPtr address, int length)
    {
        var value = ReadBytes(address, length);
        var result = Encoding.Default.GetString(value);
        return result.Replace("\0", "");
    }

    private byte[] ReadBytes(IntPtr address, int length)
    {
        IntPtr handle = this._process.Handle;
        byte[] bytes = new byte[length];
        int lpNumberOfBytesRead = 0;
        ReadProcessMemory(handle, address, bytes, bytes.Length, ref lpNumberOfBytesRead);

        return bytes;
    }
    
    private T Read<T>(MemoryPointer memoryPointer)
    {
        return memoryPointer.Offsets.Any() ? 
            ReadWithOffsets<T>(memoryPointer.Pointer, memoryPointer.Offsets.ToArray()) :
            Read<T>(memoryPointer.Pointer);
    }
    private bool Write(IntPtr address, IEnumerable<ushort> shorts)
    {
        List<byte> bytes = new List<byte>();

        foreach (ushort @ushort in shorts)
        {
            bytes.Add((byte)(@ushort & 0xFF));
            bytes.Add((byte)((@ushort >> 8) & 0xFF));
        }

        return Write(address, bytes.ToArray());

    }
    private bool Write(IntPtr address, IEnumerable<byte> bytes)
    {
        IntPtr handle = this._process.Handle;
        int lpNumberOfBytesRead = 0;
        return WriteProcessMemory(handle, address, bytes.ToArray(), bytes.Count(), ref lpNumberOfBytesRead);
    }
}