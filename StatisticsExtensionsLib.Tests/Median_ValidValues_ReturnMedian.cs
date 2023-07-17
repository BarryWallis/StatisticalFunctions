using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsExtensionsLib.Tests;

public class MedianTests
{
    [Theory]
    [InlineData(new int[] { 1, 4, 1, 5, 7, 9, 7 }, 5)]
    [InlineData(new int[] { 1, 4, 1, 5, 7, 9, 7, 6 }, 5.5)]
    public void Median_ValidValues_ReturnsMedian(int[] values, double expectedResult)
    {
        double actualResult = values.Median();

        Assert.Equal(expectedResult, actualResult);
    }

    [Fact]
    public void Median_NoValues_ThrowsArgumentException()
    {
        int[] values = Array.Empty<int>();

        _ = Assert.Throws<ArgumentException>(() => values.Median());
    }
}
