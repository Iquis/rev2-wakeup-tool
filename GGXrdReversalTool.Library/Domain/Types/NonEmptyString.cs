namespace GGXrdReversalTool.Library.Domain.Types;

public record NonEmptyString(string Value)
{
    public string Value { get; } =
        !string.IsNullOrWhiteSpace(Value)
            ? Value.Trim()
            : throw new ArgumentException("Character name must be a non-empty string");
            

    public static implicit operator string(NonEmptyString value) => value.Value;
    public static explicit operator NonEmptyString(string value) => new(value);

    public override string ToString() => Value;
}