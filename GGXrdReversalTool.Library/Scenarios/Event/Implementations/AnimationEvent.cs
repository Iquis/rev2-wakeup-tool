using GGXrdReversalTool.Library.Memory;
using GGXrdReversalTool.Library.Memory.Implementations;

namespace GGXrdReversalTool.Library.Scenarios.Event.Implementations;

public class AnimationEvent : IScenarioEvent
{
    private const string FaceDownAnimation = "CmnActFDown2Stand";
    private const string FaceUpAnimation = "CmnActBDown2Stand";
    private const string WallSplatAnimation = "CmnActWallHaritsukiGetUp";
    private const string TechAnimation = "CmnActUkemi";
    private const string CrouchBlockingAnimation = "CmnActCrouchGuardLoop";
    private const string StandBlockingAnimation = "CmnActMidGuardLoop";
    private const string HighBlockingAnimation = "CmnActHighGuardLoop";

    public IMemoryReader? MemoryReader { get; set; }

    public bool ShouldCheckWakingUp { get; set; } = true;
    public bool ShouldCheckWallSplat { get; set; } = true;
    public bool ShouldCheckAirTech { get; set; } = false;
    public bool ShouldCheckStartBlocking { get; set; } = false;
    public bool ShouldCheckBlockstunEnding { get; set; } = false;

    public bool IsValid =>
        ShouldCheckWakingUp || ShouldCheckWallSplat || ShouldCheckAirTech || ShouldCheckStartBlocking || ShouldCheckBlockstunEnding;

    private int _lastBlockstun;

    public int FramesUntilEvent(int inputReversalFrame)
    {
        if (MemoryReader is null)
            return int.MaxValue;

        var playerSide = MemoryReader.GetPlayerSide();
        var dummySide = 1 - playerSide;
        var currentDummy = MemoryReader.GetCurrentDummy();
        var animationString = MemoryReader.ReadAnimationString(dummySide);
        var animFrame = MemoryReader.GetAnimFrame(dummySide);
        var blockstun = MemoryReader.GetBlockstun(dummySide);
        var hitstop = MemoryReader.GetHitstop(dummySide);
        var freezeFrames = MemoryReader.GetSuperflashFreezeFrames(dummySide);
        var slowdownFrames = MemoryReader.GetSlowdownFrames(playerSide);
        var lastBlockstun = _lastBlockstun;
        _lastBlockstun = blockstun;

        var result = animationString switch
        {
            FaceDownAnimation when ShouldCheckWakingUp => currentDummy.FaceDownFrames - animFrame,
            FaceUpAnimation when ShouldCheckWakingUp => currentDummy.FaceUpFrames - animFrame,
            WallSplatAnimation when ShouldCheckWallSplat => currentDummy.WallSplatWakeupTiming - animFrame,
            TechAnimation when ShouldCheckAirTech => 9 - animFrame,
            _ => int.MaxValue,
        };

        if (result == int.MaxValue && ShouldCheckStartBlocking && blockstun > 0 && lastBlockstun == 0)
            result = 0;
        if (result == int.MaxValue && ShouldCheckBlockstunEnding && blockstun > 0)
            result = blockstun + hitstop - 1;

        if (result < int.MaxValue)
        {
            result += Math.Min(result, (slowdownFrames / 2) + slowdownFrames % 2);
            result -= inputReversalFrame;
            if (freezeFrames > 0)
            {
                // Avoid trying a reversal directly out of a super freeze, where everything but blocking and throwing gets dropped
                if (result + freezeFrames == 0)
                    result = int.MaxValue;
                else
                    result += freezeFrames;
            }
        }

        return result;
    }
    
}