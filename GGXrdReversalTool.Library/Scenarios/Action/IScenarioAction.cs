using GGXrdReversalTool.Library.Memory;
using GGXrdReversalTool.Library.Models;
using GGXrdReversalTool.Library.Models.Inputs;

namespace GGXrdReversalTool.Library.Scenarios.Action;

public interface IScenarioAction
{
    void Execute();
    void Tick();
    IMemoryReader? MemoryReader { get; internal set; }
    bool IsRunning { get; }
    
    SlotInput Input { get; set; }
    void Init();
    
    int SlotNumber { get; set; }
}
