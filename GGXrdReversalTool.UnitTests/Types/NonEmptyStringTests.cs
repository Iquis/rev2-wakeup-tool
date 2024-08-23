using GGXrdReversalTool.Library.Domain.Types;

namespace GGXrdReversalTool.UnitTests.Types;

public class NonEmptyStringTests
{
    [Fact]
    public void Given_The_Same_NonEmptyString_The_Objects_Should_Be_Equal()
    {
        var charName1 = new NonEmptyString("toto");
        var charName2 = new NonEmptyString("toto");

        Assert.Equal(charName1, charName2);
    }

    [Fact]
    public void Given_An_Empty_String_It_Should_Throw_ArgumentException()
    {

        Assert.Throws<ArgumentException>(() => new NonEmptyString(""));
        Assert.Throws<ArgumentException>(() => new NonEmptyString("  "));
        Assert.Throws<ArgumentException>(() => new NonEmptyString(null!));
    }
}