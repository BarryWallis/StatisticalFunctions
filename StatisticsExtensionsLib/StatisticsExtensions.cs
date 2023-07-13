using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StatisticsExtensionsLib;
/// <summary>
/// A  class that defines extension methods to calculate statistical functions for arrays or lists of numbers.
/// </summary>
public static class StatisticsExtensions
{
    /// <summary>
    /// Calculate the mean (average) of a list after removing an indicated number of the largest and smallest 
    /// values. 
    /// <para>
    /// For example, if the values are {1, 1, 3, 5, 7, 7, 9} and you want to remove the two largest and 
    /// smallest values, the remaining values are {3, 5, 7}.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the numbers in the list.</typeparam>
    /// <param name="values">The list of numbers to calculate the truncated mean on.</param>
    /// <param name="numberToRemove">The number of items to remove from the front and back of the list.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">The list of <paramref name="values"/> has no items.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The <paramref name="numberToRemove"/> must be 
    /// non-negative and less than half the number of items in the list of <paramref name="values"/>"
    /// </exception>
    public static double TruncatedMean<T>(this IEnumerable<T> values, int numberToRemove) where T : INumber<T>
    {
        #region Preconditions
        if (!values.Any())
        {
            throw new ArgumentException("No values given", nameof(values));
        }

        if (numberToRemove < 0 || numberToRemove * 2 >= values.Count())
        {
            throw new ArgumentOutOfRangeException(nameof(numberToRemove), "Must be non-negative");
        }
        #endregion

        int numberOfValues = values.Count() - (numberToRemove * 2);
        return values.Order()
                     .Skip(numberToRemove)
                     .Take(numberOfValues)
                     .Select(x=>Convert.ToDouble(x))
                     .Average();
    }

    /// <summary>
    /// Calculate the mean (average) of a list after removing an indicated percentage of the largest and
    /// smallest values. 
    /// <para>
    /// For example, if the values are {1, 1, 3, 5, 7, 7, 9, 8} and you want to remove the 24% largest and 
    /// smallest values, the remaining values are {3, 5, 7}.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the numbers in the list.</typeparam>
    /// <param name="values">The list of numbers to calculate the truncated mean on.</param>
    /// <param name="numberToRemove">
    /// The percentage of items to remove from the front and back of the list 
    /// given as a fraction (e.g., 25% would be 0.25).
    /// </param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">The list of <paramref name="values"/> has no items.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The <paramref name="percentageToRemove"/> must be 
    /// non-negative and less than .50 (50%) in the list of <paramref name="values"/>"
    /// </exception>
    public static double TruncatedMean<T>(this IEnumerable<T> values, double percentageToRemove)
        where T : INumber<T>
    {
        #region Preconditions
        if (!values.Any())
        {
            throw new ArgumentException("No values given", nameof(values));
        }

        if (percentageToRemove is < 0 or >= .5)
        {
            throw new ArgumentOutOfRangeException(
                nameof(percentageToRemove), "Must be non-negative but less than 50%");
        }
        #endregion

        int numberToRemove = (int)(values.Count() * percentageToRemove);
        return values.TruncatedMean(numberToRemove);
    }

}
