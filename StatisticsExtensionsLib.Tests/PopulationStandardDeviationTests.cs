using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsExtensionsLib.Tests;
public class PopulationStandardDeviationTests
{
    [Fact]
    public void PopulationStandardDeviation_ValidValues_ReturnsStandardDeviation()
    {
        int[] values = { 5, 10, 15, 20, 25 };
        double expectedResult = Math.Round(7.07, 2);

        double actualResult = Math.Round(values.PopulationStandardDeviation(), 2);

        Assert.Equal(expectedResult, actualResult);
    }

    [Theory]
    [InlineData(new int[] { 1 })]
    [InlineData(new int[0])]
    public void PopulationStandardDeviation_TooFewValues_ThrowsArgumentException(int[] values)
        => _ = Assert.Throws<ArgumentException>(() => values.PopulationStandardDeviation());

}
