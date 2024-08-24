using GGXrdReversalTool.Library.Domain.Characters;

namespace GGXrdReversalTool.UnitTests.Characters;

public class CharacterMoveTests
{
    [Fact]
    public void Moves_With_Same_Properties_Should_Be_Equal()
    {
        Assert.Equal(new CharacterMove("name","input"), new CharacterMove("name","input"));
    }
}