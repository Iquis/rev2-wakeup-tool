using GGXrdReversalTool.Library.Memory;

namespace GGXrdReversalTool.Library.Scenarios.Event.Implementations;

public class ComboEvent : IScenarioEvent
{
    public int MinComboCount { get; set; } = 1;
    public int MaxComboCount { get; set; } = 5;

    public IMemoryReader? MemoryReader { get; set; }

    private int _oldComboCount;

    public bool IsValid => MinComboCount <= MaxComboCount;

    public int FramesUntilEvent(int inputReversalFrame)
    {
        if (MemoryReader is null)
            return int.MaxValue;

        var comboCount = MemoryReader.GetComboCount(1 - MemoryReader.GetPlayerSide());

        var result = comboCount >= MinComboCount && comboCount <= MaxComboCount && _oldComboCount != comboCount ? 0 : int.MaxValue;

        _oldComboCount = comboCount;

        return result;
    }

}