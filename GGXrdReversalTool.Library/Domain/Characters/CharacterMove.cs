using GGXrdReversalTool.Library.Domain.Types;

namespace GGXrdReversalTool.Library.Domain.Characters;

public record CharacterMove(NonEmptyString Name, NonEmptyString Input)
{
    public CharacterMove(string name, string input)
        : this(new NonEmptyString(name), new NonEmptyString(input)) { }
}
