namespace GGXrdReversalTool.Library.Scenarios.Event;

public class EventAnimationInfo
{
    public AnimationEventTypes EventType { get; set; } = AnimationEventTypes.None;
    public int Delay { get; set; } = 0;

    public EventAnimationInfo()
    :this(AnimationEventTypes.None, 0)
    {
        
    }

    public EventAnimationInfo( AnimationEventTypes animationEventType)
    :this(animationEventType,0)
    {
        
    }

    public EventAnimationInfo( AnimationEventTypes animationEventType, int delay)
    {
        EventType = animationEventType;
        Delay = delay;
    }
}