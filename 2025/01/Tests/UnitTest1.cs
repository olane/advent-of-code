using Xunit;
using Day1;

namespace Tests;

public class UnitTest1
{
    [Theory]
    [InlineData(50, "R50", 0, 1)]
    [InlineData(50, "R49", 99, 0)]
    [InlineData(50, "R51", 1, 1)]
    [InlineData(50, "R151", 1, 2)]
    [InlineData(50, "R149", 99, 1)]
    [InlineData(50, "R150", 0, 2)]
    [InlineData(0, "R1", 1, 0)]
    [InlineData(0, "R100", 0, 1)]
    [InlineData(0, "R150", 50, 1)]
    [InlineData(0, "R200", 0, 2)]
    [InlineData(0, "L1", 99, 0)]
    [InlineData(0, "L100", 0, 1)]
    [InlineData(0, "L150", 50, 1)]
    [InlineData(0, "L200", 0, 2)]
    [InlineData(0, "L300", 0, 3)]
    [InlineData(50, "L300", 50, 3)]
    [InlineData(50, "L50", 0, 1)]
    [InlineData(50, "L49", 1, 0)]
    [InlineData(50, "L51", 99, 1)]
    public void Test1(int before, string input, int after, int zeros)
    {
        (var afterResult, var zerosResult) = Mover.Move2(before, input);
        Assert.Equal(after, afterResult);
        Assert.Equal(zeros, zerosResult);
    }
}
