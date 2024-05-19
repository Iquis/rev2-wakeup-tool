using GGXrdReversalTool.Library.Memory;

namespace GGXrdReversalTool.Library.Scenarios.Frequency.Implementations;

public class PercentageFrequency : IScenarioFrequency
{
    private readonly Random _random = new();
    private int _percentage = 100;

    public int Percentage
    {
        get => _percentage;
        set => _percentage = Math.Max(Math.Min(value, 100), 0);
    }


    public bool ShouldHappen()
    {
        return _random.Next(0, 101) <= _percentage;
    }

    public IMemoryReader? MemoryReader { get; set; }
}