using System.Numerics;

using Xunit.Sdk;

namespace StatisticsExtensionsLib.Tests;

public class TruncatedMeanWithNumberToRemoveTests
{
    [Theory]
    [InlineData(new int[] { 5, 3, 7, 1, 1, 7, 9 }, 2, ((3.0 + 5.0 + 7.0) / 3.0))]
    [InlineData(new int[] { 5, 3, 7 }, 0, (3.0 + 5.0 + 7.0) / 3.0)]
    [InlineData(new int[] { 3, 6 }, 0, (3.0 + 6.0) / 2.0)]
    public void TruncatedMean_ValidIntValuesAndNumberToRemove_ReturnsTruncatedMean(IEnumerable<int> intValues,
                                                                                   int numberToRemove,
                                                                                   double expectedResult)
    {
        double[] doubleValues = intValues.Select(v=>(double)v).ToArray();

        double actualResultInt = intValues.TruncatedMean(numberToRemove);
        double actualResultDouble = doubleValues.TruncatedMean(numberToRemove);

        Assert.Equal(expectedResult, actualResultInt);
        Assert.Equal(expectedResult, actualResultDouble);
    }

    [Fact]
    public void TruncatedMean_ValuesCountIsZero_ThrowsInvalidArgumentException()
    {
        int[] values = Array.Empty<int>();

        _ = Assert.Throws<ArgumentException>(() => values.TruncatedMean(0));
    }

    [Fact]
    public void TruncatedMean_NumberOfResultsIsNegative_ThrowsArgumentOutOfRangeException()
    {
        int[] values = new int[] { 1, 2, 3 };

        _ = Assert.Throws<ArgumentOutOfRangeException>(() => values.TruncatedMean(-1));
    }

    [Theory]
    [InlineData(new int[] { 1, 2, 3, 4 }, 2)]
    [InlineData(new int[] { 1, 2, 3, 4 }, 3)]
    public void TruncatedMean_NumberOfResultsLeavesNoValuesLeft_ThrowsArgumentOutOfRangeException
                (IEnumerable<int> values, int numberToRemove) 
        => _ = Assert.Throws<ArgumentOutOfRangeException>(() => values.TruncatedMean(numberToRemove));
}
