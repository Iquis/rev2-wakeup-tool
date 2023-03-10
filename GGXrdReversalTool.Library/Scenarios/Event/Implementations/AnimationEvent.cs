using GGXrdReversalTool.Library.Memory;

namespace GGXrdReversalTool.Library.Scenarios.Event.Implementations;

public class AnimationEvent : IScenarioEvent
{
    private const string FaceDownAnimation = "CmnActFDown2Stand";
    private const string FaceUpAnimation = "CmnActBDown2Stand";
    private const string WallSplatAnimation = "CmnActWallHaritsukiGetUp";
    private const string TechAnimation = "CmnActUkemi";
    private const string CrouchBlockingAnimation = "CmnActCrouchGuard";
    private const string StandBlockingAnimation = "CmnActMidGuardLoop";
    private const string HighBlockingAnimation = "CmnActHighGuardLoop";

    public IMemoryReader MemoryReader { get; set; }

    public bool ShouldCheckWakingUp { get; set; } = true;
    public bool ShouldCheckWallSplat { get; set; } = true;
    public bool ShouldCheckAirTech { get; set; } = false;
    public bool ShouldCheckStartBlocking { get; set; } = false;

    public bool IsValid =>
        ShouldCheckWakingUp || ShouldCheckWallSplat || ShouldCheckAirTech || ShouldCheckStartBlocking;

    public AnimationEventTypes CheckEvent()
    {
        var animationString = MemoryReader.ReadAnimationString(2);

        return animationString switch
        {
            FaceDownAnimation when ShouldCheckWakingUp => AnimationEventTypes.KDFaceDown,
            FaceUpAnimation when ShouldCheckWakingUp => AnimationEventTypes.KDFaceUp,
            WallSplatAnimation when ShouldCheckWallSplat => AnimationEventTypes.WallSplat,
            TechAnimation when ShouldCheckAirTech => AnimationEventTypes.Tech,
            CrouchBlockingAnimation or StandBlockingAnimation or HighBlockingAnimation when ShouldCheckStartBlocking => AnimationEventTypes.Blocking,
            _ => AnimationEventTypes.None
        };
    }
    
}