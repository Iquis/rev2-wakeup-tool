namespace GGXrdReversalTool.Library.Domain.Frames;

public readonly record struct FrameCount(int Value)
{
    public int Value { get; } = Value >= 0 ? Value : throw new ArgumentException("Value must be a positive integer");

    public static implicit operator int(FrameCount value) => value.Value;
}