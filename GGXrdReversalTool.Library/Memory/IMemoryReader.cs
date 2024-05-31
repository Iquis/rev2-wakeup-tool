using System.Diagnostics;
using GGXrdReversalTool.Library.Characters;
using GGXrdReversalTool.Library.Memory.Pointer;
using GGXrdReversalTool.Library.Models;
using GGXrdReversalTool.Library.Models.Inputs;

namespace GGXrdReversalTool.Library.Memory;

public interface IMemoryReader
{
    string ReadAnimationString(int player);
    int FrameCount();
    Character GetCurrentDummy();
    public bool SetDummyPlayback(int slotNumber, int inputIndex, int startingSide);
    bool WriteInputInSlot(int slotNumber, SlotInput slotInput);
    int GetComboCount(int player);
    int GetBlockstun(int player);
    int GetHitstop(int player);
    public int GetFacing(int player);
    int GetPlayerSide();
    bool IsTrainingMode();
    public bool IsWorldInTick();
    public uint GetEngineTickCount();

    Process Process { get; }
    SlotInput ReadInputFromSlot(int slotNumber);
}