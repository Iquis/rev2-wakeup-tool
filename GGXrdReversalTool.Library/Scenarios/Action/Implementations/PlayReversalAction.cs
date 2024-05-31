using System.Runtime.InteropServices;
using GGXrdReversalTool.Library.Configuration;
using GGXrdReversalTool.Library.Input;
using GGXrdReversalTool.Library.Logging;
using GGXrdReversalTool.Library.Memory;
using GGXrdReversalTool.Library.Models;
using GGXrdReversalTool.Library.Models.Inputs;

namespace GGXrdReversalTool.Library.Scenarios.Action.Implementations;

public class PlayReversalAction : IScenarioAction
{
    public IMemoryReader? MemoryReader { get; set; }
    public SlotInput Input { get; set; } = null!;

    public void Init()
    {
        if (MemoryReader == null)
        {
            return;
        }

        MemoryReader.WriteInputInSlot(SlotNumber, Input);
    }
    public int SlotNumber { get; set; } = 1;

    public void Execute()
    {
        if (MemoryReader == null)
        {
            return;
        }
        LogManager.Instance.WriteLine("PlayReversalAction Execute!");
        MemoryReader.SetDummyPlayback(SlotNumber, 0, MemoryReader.GetFacing(1 - MemoryReader.GetPlayerSide()));
    }
}