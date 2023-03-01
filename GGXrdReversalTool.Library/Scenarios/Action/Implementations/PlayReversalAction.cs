using GGXrdReversalTool.Library.Configuration;
using GGXrdReversalTool.Library.Input;
using GGXrdReversalTool.Library.Logging;
using GGXrdReversalTool.Library.Memory;
using GGXrdReversalTool.Library.Models;
using GGXrdReversalTool.Library.Models.Inputs;
using GGXrdReversalTool.Library.Replay;
using GGXrdReversalTool.Library.Replay.Implementations;

namespace GGXrdReversalTool.Library.Scenarios.Action.Implementations;

public class PlayReversalAction : IScenarioAction
{
    public IMemoryReader? MemoryReader { get; set; }
    public SlotInput Input { get; set; } = null!;

    private IReplayTrigger? _replayTrigger;


    public void Init()
    {
        if (MemoryReader == null)
        {
            return;
        }

        InitReplayTrigger();


        MemoryReader.WriteInputInSlot(SlotNumber, Input);
    }

    public int SlotNumber { get; set; } = 1;

    public void Execute()
    {
        LogManager.Instance.WriteLine("PlayReversalAction Execute!");
        _replayTrigger?.Trigger();
    }


    private void InitReplayTrigger()
    {
        var replayTriggerTypeConfig = ReversalToolConfiguration.Get("ReplayTriggerType");

        if (!Enum.TryParse(replayTriggerTypeConfig, false, out ReplayTriggerTypes replayTriggerType))
        {
            replayTriggerType = ReplayTriggerTypes.AsmInjection;
        }

        switch (replayTriggerType)
        {
            case ReplayTriggerTypes.AsmInjection:
            {
                // _replayTrigger = new AsmInjectionReplayTrigger();
                //TODO fix asm injection
                var replayKeyStroke = GetReplayKeyStroke();
                _replayTrigger = new KeystrokeReplayTrigger(replayKeyStroke, MemoryReader.Process);
            }
                break;
            case ReplayTriggerTypes.Keystroke:
            {
                var replayKeyStroke = GetReplayKeyStroke();
                _replayTrigger = new KeystrokeReplayTrigger(replayKeyStroke, MemoryReader.Process);
            }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private DirectXKeyStrokes GetReplayKeyStroke()
    {
        //TODO handle when replayCode is not a letter
        int replayKeyCode = MemoryReader.GetReplayKeyCode(1);
        char replayKey = (char)replayKeyCode;

        if (!Enum.TryParse($"DIK_{replayKey}", out DirectXKeyStrokes stroke))
        {
            stroke = DirectXKeyStrokes.DIK_P;
        }

        return stroke;
    }

    
}