using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
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
                     .Select(x => Convert.ToDouble(x))
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

    /// <summary>
    /// Return the middle value from a list of numbers.
    /// </summary>
    /// <typeparam name="T">The type of the list. Must support INumber.</typeparam>
    /// <param name="values">The list of values.</param>
    /// <returns>
    /// If the list has an odd number of values, returns the middle value; otherwise returns the 
    /// average of the two middle values.
    /// </returns>
    /// <exception cref="ArgumentException">The list is empty.</exception>
    public static double Median<T>(this IEnumerable<T> values) where T : INumber<T>
    {
        #region Preconditions
        if (!values.Any())
        {
            throw new ArgumentException("No values given", nameof(values));
        }
        #endregion

        IEnumerable<T> sortedValues = values.Order();
        return values.Count() % 2 == 0
            ? sortedValues.Skip((values.Count() / 2) - 1)
                               .Take(2)
                               .Select(x => Convert.ToDouble(x))
                               .Average()
            : Convert.ToDouble(sortedValues.Skip(values.Count() / 2).First());
    }

    /// <summary>
    /// Return a list of the numbers whose value occurs most often. 
    /// </summary>
    /// <typeparam name="T">The type of the numbers in the list.</typeparam>
    /// <param name="values">The list of numbers.</param>
    /// <returns>A list containing the number(s) that occur most often.</returns>
    /// <exception cref="ArgumentException">The list was empty.</exception>
    public static IEnumerable<T> Mode<T>(this IEnumerable<T> values) where T : INumber<T>
    {
        #region Preconditions
        if (!values.Any())
        {
            throw new ArgumentException("No values given", nameof(values));
        }
        #endregion

        IDictionary<T, int> entries = new Dictionary<T, int>();
        foreach (T value in values)
        {
            if (entries.ContainsKey(value))
            {
                entries[value]++;
            }
            else
            {
                entries.Add(value, 1);
            }
        }

        IOrderedEnumerable<KeyValuePair<T, int>> sortedEntries
            = entries.OrderByDescending(kvp => kvp.Value);
        IEnumerable<T> results
            = sortedEntries.Where(kvp => kvp.Value == sortedEntries.First().Value)
                           .Select(kvp => kvp.Key);
        return results;
    }

    /// <summary>
    /// Return the sample standard deviation of a list of numbers.
    /// </summary>
    /// <typeparam name="T">The type of numbers in the list. Must implement INumber.</typeparam>
    /// <param name="values">The list of numbers.</param>
    /// <returns>The sample standard deviation.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="values"/> must have more than one value.
    /// </exception>
    public static double SampleStandardDeviation<T>(this IEnumerable<T> values) where T : INumber<T>
    {
        #region Preconditions
        if (values.Count() <= 1)
        {
            throw new ArgumentException("Must have more than one value", nameof(values));
        }
        #endregion

        double average = values.Select(x => Convert.ToDouble(x)).Average();
        double sum = 0.0;
        foreach (T item in values)
        {
            sum += Math.Pow(Convert.ToDouble(item) - average, 2);
        }
        return Math.Sqrt(1.0 / (values.Count() - 1) * sum);
    }

    /// <summary>
    /// Return the population standard deviation of a list of numbers.
    /// </summary>
    /// <typeparam name="T">The type of numbers in the list. Must implement INumber.</typeparam>
    /// <param name="values">The list of numbers.</param>
    /// <returns>The population standard deviation.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="values"/> must have more than one value.
    /// </exception>
    public static double PopulationStandardDeviation<T>(this IEnumerable<T> values) where T : INumber<T>
    {
        #region Preconditions
        if (values.Count() <= 1)
        {
            throw new ArgumentException("Must have more than one value", nameof(values));
        }
        #endregion

        double average = values.Select(x => Convert.ToDouble(x)).Average();
        double sum = 0.0;
        foreach (T item in values)
        {
            sum += Math.Pow(Convert.ToDouble(item) - average, 2);
        }
        return Math.Sqrt(1.0 / values.Count() * sum);
    }
}
