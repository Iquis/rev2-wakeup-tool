using GGXrdReversalTool.Library.Models.Inputs;

namespace GGXrdReversalTool.UnitTests;

public class SlotInputTests
{
    [Theory]
    [InlineData("1", new ushort[] { 0b0110 })]
    [InlineData("2", new ushort[] { 0b0010 })]
    [InlineData("3", new ushort[] { 0b1010 })]
    [InlineData("4", new ushort[] { 0b0100 })]
    [InlineData("5", new ushort[] { 0b0000 })]
    [InlineData("6", new ushort[] { 0b1000 })]
    [InlineData("7", new ushort[] { 0b0101 })]
    [InlineData("8", new ushort[] { 0b0001 })]
    [InlineData("9", new ushort[] { 0b1001 })]
    [InlineData("5P", new ushort[] { 0x10 })]
    [InlineData("5K", new ushort[] { 0x20 })]
    [InlineData("5S", new ushort[] { 0x40 })]
    [InlineData("5H", new ushort[] { 0x80 })]
    [InlineData("5D", new ushort[] { 0x100 })]
    [InlineData("5PK", new ushort[] { 0x30 })]
    [InlineData("5PKS", new ushort[] { 0x70 })]
    [InlineData("5PKSH", new ushort[] { 0xF0 })]
    [InlineData("5PKSHD", new ushort[] { 0x1F0 })]
    [InlineData("6,2,3H", new ushort[] { 0b1000, 0b0010, 0x8A })]
    [InlineData("6,2,!3H", new ushort[] { 0b1000, 0b0010, 0x8A })]

    [InlineData("5p", new ushort[] { 0x10 })]
    [InlineData("5k", new ushort[] { 0x20 })]
    [InlineData("5s", new ushort[] { 0x40 })]
    [InlineData("5h", new ushort[] { 0x80 })]
    [InlineData("5d", new ushort[] { 0x100 })]
    [InlineData("5pk", new ushort[] { 0x30 })]
    [InlineData("5pks", new ushort[] { 0x70 })]
    [InlineData("5pksh", new ushort[] { 0xF0 })]
    [InlineData("5pkshd", new ushort[] { 0x1F0 })]
    [InlineData("6,2,3h", new ushort[] { 0b1000, 0b0010, 0x8A })]
    [InlineData("6,2,!3h", new ushort[] { 0b1000, 0b0010, 0x8A })]
    [InlineData("6,5pk*7", new ushort[] { 0b1000, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 })]

    public void SlotInput_Input_Parse_Test(string input, ushort[] content)
    {
        var slotInput = new SlotInput(input);


        Assert.Equal(content, slotInput.Content.ToArray());
    }

    [Theory]
    [InlineData("6,2,3h",-1)]
    [InlineData("6,2,!3h",2)]
    [InlineData("6,2,!3h*7",2)]
    [InlineData("2*40,!8H",40)]
    public void SlotInput_ReversalFrameIndex_Test(string input, int result)
    {
        var slotInput = new SlotInput(input);
        Assert.Equal(result, slotInput.ReversalFrameIndex);
    }

    [Theory]
    [InlineData("6,5*7",new []{"6","5","5","5","5","5","5","5"})]
    [InlineData("6*1,5*7",new []{"6","5","5","5","5","5","5","5"})]
    [InlineData("6*0,5*7",new []{"5","5","5","5","5","5","5"})]
    public void SlotInput_ExpandedInputList_Test(string rawInputText, IEnumerable<string> expected)
    {
        var slotInput1 = new SlotInput(rawInputText);
        
        Assert.Equal(expected, slotInput1.ExpandedInputList);
        
    }
    
    
    [Theory]
    [InlineData("6,5*7", new []{"6","5*7"})]
    [InlineData("6*1,5*7", new []{"6","5*7"})]
    [InlineData("6*0,5*7", new []{"5*7"})]
    public void SlotInput_CondensedInputList_Test(string rawInputText, IEnumerable<string> expected)
    {
        var slotInput1 = new SlotInput(rawInputText);
        
        Assert.Equal(expected, slotInput1.CondensedInputListText);
        
    }
}