using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsExtensionsLib.Tests;
public class SampleStandardDeviationTests
{
    [Fact]
    public void SampleStandardDeviation_ValidValues_ReturnsStandardDeviation()
    {
        int[] values = { 5, 10, 15, 20, 25 };
        double expectedResult = Math.Round(7.91, 2);

        double actualResult = Math.Round(values.SampleStandardDeviation(), 2);

        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData(new int[] { 1 })]
    [InlineData(new int[0])]
    public void SampleStandardDeviation_TooFewValues_ThrowsArgumentException(int[] values) 
        => _ = Assert.Throws<ArgumentException>(() => values.SampleStandardDeviation());
}
