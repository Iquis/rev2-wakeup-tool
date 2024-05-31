using GGXrdReversalTool.Library.Memory;

namespace GGXrdReversalTool.Library.Scenarios.Event.Implementations;

public class ComboEvent : IScenarioEvent
{
    public int MinComboCount { get; set; } = 1;
    public int MaxComboCount { get; set; } = 5;

    public IMemoryReader MemoryReader { get; set; }

    private int _oldComboCount;

    public bool IsValid => MinComboCount <= MaxComboCount;

    public EventAnimationInfo CheckEvent()
    {
        var comboCount = MemoryReader.GetComboCount(MemoryReader.GetPlayerSide());

        if (comboCount >= MinComboCount && comboCount<= MaxComboCount && _oldComboCount != comboCount)
        {
            _oldComboCount = comboCount;
            return new EventAnimationInfo(AnimationEventTypes.Combo);
        }

        _oldComboCount = comboCount;

        return new EventAnimationInfo(AnimationEventTypes.None);
    }

}