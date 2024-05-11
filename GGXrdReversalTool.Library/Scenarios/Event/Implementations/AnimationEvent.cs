using GGXrdReversalTool.Library.Memory;

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

    public IMemoryReader MemoryReader { get; set; }

    public bool ShouldCheckWakingUp { get; set; } = true;
    public bool ShouldCheckWallSplat { get; set; } = true;
    public bool ShouldCheckAirTech { get; set; } = false;
    public bool ShouldCheckStartBlocking { get; set; } = false;
    public bool ShouldCheckBlockstunEnding { get; set; } = false;

    public bool IsValid =>
        ShouldCheckWakingUp || ShouldCheckWallSplat || ShouldCheckAirTech || ShouldCheckStartBlocking || ShouldCheckBlockstunEnding;

    private string _lastAnimationString = ""; 

    public EventAnimationInfo CheckEvent()
    {
        var animationString = MemoryReader.ReadAnimationString(1 - MemoryReader.GetPlayerSide());

        var result = animationString switch
        {
            FaceDownAnimation when ShouldCheckWakingUp => new EventAnimationInfo(AnimationEventTypes.KDFaceDown),
            FaceUpAnimation when ShouldCheckWakingUp => new EventAnimationInfo(AnimationEventTypes.KDFaceUp),
            WallSplatAnimation when ShouldCheckWallSplat => new EventAnimationInfo(AnimationEventTypes.WallSplat),
            TechAnimation when ShouldCheckAirTech => new EventAnimationInfo(AnimationEventTypes.Tech),
            CrouchBlockingAnimation or StandBlockingAnimation or HighBlockingAnimation when ShouldCheckStartBlocking && _lastAnimationString is not (CrouchBlockingAnimation or StandBlockingAnimation or HighBlockingAnimation) => new EventAnimationInfo(AnimationEventTypes.StartBlocking, 0),
            CrouchBlockingAnimation or StandBlockingAnimation or HighBlockingAnimation when ShouldCheckBlockstunEnding => new EventAnimationInfo(AnimationEventTypes.EndBlocking, MemoryReader.GetBlockstun(1 - MemoryReader.GetPlayerSide()) + MemoryReader.GetHitstop(1 - MemoryReader.GetPlayerSide())),
            _ => new EventAnimationInfo()
        };

        //TODO Refactor pour envoyer l'event uniquement quand changement

        _lastAnimationString = animationString ;

        return result;
    }
    
}