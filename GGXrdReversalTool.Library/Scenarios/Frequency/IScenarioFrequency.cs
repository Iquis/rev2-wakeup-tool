using GGXrdReversalTool.Library.Memory;

namespace GGXrdReversalTool.Library.Scenarios.Frequency;

public interface IScenarioFrequency
{
    bool ShouldHappen();
    IMemoryReader? MemoryReader { get; internal set; }
}