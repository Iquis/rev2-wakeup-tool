namespace GGXrdReversalTool.Library.Domain.Types;

public record NonEmptyString(string Value)
{
    private string Value { get; } =
        !string.IsNullOrWhiteSpace(Value)
            ? Value.Trim()
            : throw new ArgumentException("Character name must be a non-empty string");
            

    public static implicit operator NonEmptyString(string value) => new(value);

    public override string ToString() => Value;
}