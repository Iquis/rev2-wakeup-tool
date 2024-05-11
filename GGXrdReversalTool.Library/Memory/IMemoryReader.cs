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
    bool WriteInputInSlot(int slotNumber, SlotInput slotInput);
    int GetComboCount(int player);
    int GetReplayKeyCode();
    int GetBlockstun(int player);
    int GetPlayerSide();
    bool IsTrainingMode();
    public bool IsWorldInTick();
    public uint GetEngineTickCount();

    Process Process { get; }
    SlotInput ReadInputFromSlot(int slotNumber);
}