namespace GGXrdReversalTool.Library.Domain.Types;

public readonly record struct NonEmptyString(string Value)
{
    public string Value { get; } =
        !string.IsNullOrWhiteSpace(Value)
            ? Value.Trim()
            : throw new ArgumentException("Value cannot be empty", nameof(Value));

    public static implicit operator string(NonEmptyString x) => x.Value;
    public static explicit operator NonEmptyString(string x) => new(x);

    public override string ToString() => Value;
}