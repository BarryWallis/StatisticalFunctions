using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatisticsExtensionsLib.Tests;
public class TruncatedMeanWithPercentageToRemoveTests
{
    [Theory]
    [InlineData(new int[] { 5, 3, 7, 1, 1, 7, 9 }, 2.0 / 7.0, ((3.0 + 5.0 + 7.0) / 3.0))]
    [InlineData(new int[] { 5, 3, 7 }, 0.0, (3.0 + 5.0 + 7.0) / 3.0)]
    [InlineData(new int[] { 3, 6 }, 0.0, (3.0 + 6.0) / 2.0)]
    public void TruncatedMean_ValidIntValuesAndPercentageToRemove_ReturnsTruncatedMean
    (IEnumerable<int> intValues, double percentageToRemove, double expectedResult)
    {
        double[] doubleValues = intValues.Select(v => (double)v).ToArray();

        double actualResultInt = intValues.TruncatedMean(percentageToRemove);
        double actualResultDouble = doubleValues.TruncatedMean(percentageToRemove);

        Assert.Equal(expectedResult, actualResultInt);
        Assert.Equal(expectedResult, actualResultDouble);
    }

    [Fact]
    public void TruncatedMean_ValuesCountIsZeroPercentage_ThrowsInvalidArgumentException()
    {
        int[] values = Array.Empty<int>();

        _ = Assert.Throws<ArgumentException>(() => values.TruncatedMean(0.0));
    }

    [Fact]
    public void TruncatedMean_NumberOfResultsIsNegativePercentage_ThrowsArgumentOutOfRangeException()
    {
        int[] values = new int[] { 1, 2, 3 };

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => values.TruncatedMean(-1.0));
    }

    [Theory]
    [InlineData(new int[] { 1, 2, 3, 4 }, .5)]
    [InlineData(new int[] { 1, 2, 3, 4 }, .75)]
    public void TruncatedMean_NumberOfResultsLeavesNoValuesLeftPercentage_ThrowsArgumentOutOfRangeException
                (IEnumerable<int> values, double percentageToRemove)
        => _ = Assert.Throws<ArgumentOutOfRangeException>(() => values.TruncatedMean(percentageToRemove));
}
