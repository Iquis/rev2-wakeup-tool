using GGXrdReversalTool.Library.Extensions;

namespace GGXrdReversalTool.UnitTests;

public class LinqExtensionsTests
{
    [Fact]
    public void FirstIndexOf_Tests()
    {
        var result = new[] { 1, 2, 3, 4, 5, 6 }.FirstIndexOf(item => item > 2);
        var result2 = new[] { 1, 2, 3, 4, 5, 6 }.FirstIndexOf(item => item > 10);
        
        Assert.Equal(2, result);
        Assert.Equal(-1, result2);
        
    }

    [Fact]
    public void FirstIndexOf_Throws_When_Source_Is_Null()
    {
        
        Assert.Throws<ArgumentException>(() =>
        {
            IEnumerable<int> source = null!;

            source.FirstIndexOf(x => x > 0);
        });
    }

    [Fact]
    public void FirstIndexOf_Throws_When_Predicate_Is_Null()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new[] { 1, 2, 3, 4, 5 }.FirstIndexOf(null!);
        });
    }


    [Fact]
    public void GroupWhile_Returns_RightValue()
    {
        IEnumerable<Tuple<string,int>> source = new[]
        {
            new Tuple<string,int>("A", 0),
            new Tuple<string,int>("A", 1),
            new Tuple<string,int>("B", 2),
            new Tuple<string,int>("C", 3),
            new Tuple<string,int>("C", 4),
            new Tuple<string,int>("C", 5)
        };


        IEnumerable<IEnumerable<Tuple<string, int>>> result = source.GroupWhile((prev, next) => prev.Item1 == next.Item1);

        IEnumerable<IEnumerable<Tuple<string, int>>> expected = new[]
        {
            new []{ new Tuple<string,int>("A", 0), new Tuple<string,int>("A", 1)},
            new []{ new Tuple<string,int>("B", 2),},
            new []{ new Tuple<string,int>("C", 3), new Tuple<string,int>("C", 4), new Tuple<string,int>("C", 5)},
        };
        
        Assert.Equal(expected, result);

    }
    
    [Fact]
    public void GroupWhile_Returns_Empty_When_Source_Is_Empty()
    {
        var result = Enumerable.Empty<int>().GroupWhile((_,_) => true);

        Assert.Equal(Enumerable.Empty<IEnumerable<int>>(), result);
    }
}