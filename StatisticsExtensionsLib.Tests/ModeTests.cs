using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.UI.Xaml.Automation;

namespace StatisticsExtensionsLib.Tests;
public class ModeTests
{
    [Theory]
    [InlineData(new int[] {1, 3, 7, 3, 2}, new int[] {3})]
    [InlineData(new int[] {1, 3, 7, 3, 1}, new int[] {1, 3})]
    public void Mode_ValidValues_ReturnsMode(int[] values, int[] expectedResults)
    {
        IEnumerable<int> sortedExpectedResults = expectedResults.Order();

        IEnumerable<int> sortedActualResults = values.Mode().Order();

        Assert.Equal(sortedExpectedResults, sortedActualResults);
    }

    [Fact]
    public void Mode_EmptyValues_ThrowsArgumentException()
    {
        int[] values = Array.Empty<int>();

        _ = Assert.Throws<ArgumentException>(() => values.Mode());
    }
}
