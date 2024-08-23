namespace GGXrdReversalTool.Library.Domain.Characters;

public record CharacterName(string Value)
{
    private string Value { get; } =
        !string.IsNullOrWhiteSpace(Value)
            ? Value.Trim()
            : throw new ArgumentException("Character name must be a non-empty string");
            

    public static implicit operator CharacterName(string value) => new(value);

    public override string ToString() => Value;
}