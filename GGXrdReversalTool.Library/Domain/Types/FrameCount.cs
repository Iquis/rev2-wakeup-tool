namespace GGXrdReversalTool.Library.Domain.Types;

public readonly record struct FrameCount(int Value)
{
    public int Value { get; } =
        Value >= 0
            ? Value
            : throw new ArgumentException("Frame count must be a positive integer", nameof(Value));

    public static implicit operator int(FrameCount value) => value.Value;
    
}