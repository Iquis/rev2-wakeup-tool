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

    public int FramesUntilEvent()
    {
        if (MemoryReader is null)
            return int.MaxValue;

        var dummySide = 1 - MemoryReader.GetPlayerSide();
        var currentDummy = MemoryReader.GetCurrentDummy();
        var animationString = MemoryReader.ReadAnimationString(dummySide);
        var animFrame = MemoryReader.GetAnimFrame(dummySide);
        var blockstun = MemoryReader.GetBlockstun(dummySide);
        var hitstop = MemoryReader.GetHitstop(dummySide);
        var lastBlockstun = _lastBlockstun;
        _lastBlockstun = blockstun;

        switch (animationString)
        {
            case FaceDownAnimation when ShouldCheckWakingUp:
                return currentDummy.FaceDownFrames - animFrame;
            case FaceUpAnimation when ShouldCheckWakingUp:
                return currentDummy.FaceUpFrames - animFrame;
            case WallSplatAnimation when ShouldCheckWallSplat:
                return currentDummy.WallSplatWakeupTiming - animFrame;
            case TechAnimation when ShouldCheckAirTech:
                return 9 - animFrame;
        };

        if (ShouldCheckStartBlocking && blockstun > 0 && lastBlockstun == 0)
            return 0;
        if (ShouldCheckBlockstunEnding && blockstun > 0)
            return blockstun + hitstop - 1;

        return int.MaxValue;
    }
    
}