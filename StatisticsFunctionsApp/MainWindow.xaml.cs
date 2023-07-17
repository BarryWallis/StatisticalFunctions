using System;
using System.Runtime.CompilerServices;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using static System.Runtime.InteropServices.JavaScript.JSType;

using Windows.Foundation.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using StatisticsExtensionsLib;
using WinRT.Interop;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using System.Xml.Schema;
using Microsoft.UI.Xaml.Shapes;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StatisticsFunctionsApp;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    private int[] _valuesArray = null;

    public MainWindow()
    {
        InitializeComponent();

        // Get the current window handle
        IntPtr hWnd = WindowNative.GetWindowHandle(this);

        // Get the window ID
        WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);

        // Get the AppWindow object
        AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

        appWindow.Changed += async (s, e) => await RedrawHistogram(_valuesArray);
    }

    private async void GoButton_Click(object sender, RoutedEventArgs e)
    {
        bool isInputValid = true;
        if (!int.TryParse(NumberOfValuesTextBox.Text, out int numberOfValues)
            || numberOfValues <= 1)
        {
            isInputValid = false;
            ContentDialog dialog = new()
            {
                // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                XamlRoot = Content.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "Error!",
                CloseButtonText = "Ok",
                DefaultButton = ContentDialogButton.Close,
                Content = "Invalid number of _valuesArray"
            };
            _ = await dialog.ShowAsync();
        }

        if (!double.TryParse(DiscardFractionTextBox.Text, out double discardFraction)
            || discardFraction < 0.0
            || discardFraction >= .5)
        {
            isInputValid = false;
            ContentDialog dialog = new()
            {
                // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                XamlRoot = Content.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "Error!",
                CloseButtonText = "Ok",
                DefaultButton = ContentDialogButton.Close,
                Content = "Invalid discard fraction"
            };
            _ = await dialog.ShowAsync();
        }

        if (!isInputValid)
        {
            return;
        }

        _valuesArray = new int[numberOfValues];
        Random random = new();
        for (int i = 0; i < numberOfValues; i++)
        {
            _valuesArray[i] = random.Next(6) + 1 + random.Next(6) + 1;
        }

        ValuesListBox.Items.Clear();
        foreach (int value in _valuesArray)
        {
            ValuesListBox.Items.Add(value);
        }

        SortedListBox.Items.Clear();
        foreach (int value in _valuesArray.Order())
        {
            SortedListBox.Items.Add(value);
        }

        List<int> valuesList = _valuesArray.ToList();
        MinimumArrayTextBox.Text = _valuesArray.Order().First().ToString();
        MinimumListTextBox.Text = valuesList.Order().First().ToString();
        MaximumArrayTextBox.Text = _valuesArray.OrderDescending().First().ToString();
        MaximumListTextBox.Text = valuesList.OrderDescending().First().ToString();
        MeanArrayTextBox.Text = _valuesArray.Average().ToString();
        MeanListTextBox.Text = valuesList.Average().ToString();
        TruncatedMeanArrayTextBox.Text = _valuesArray.TruncatedMean(discardFraction).ToString();
        TruncatedMeanListTextBox.Text = valuesList.TruncatedMean(discardFraction).ToString();
        MedianArrayTextBox.Text = _valuesArray.Median().ToString();
        MedianListTextBox.Text = valuesList.Median().ToString();
        ModeArrayTextBox.Text = string.Join(" ", _valuesArray.Mode());
        ModeListTextBox.Text = string.Join(" ", valuesList.Mode());
        StddevSampleArrayTextBox.Text = _valuesArray.SampleStandardDeviation().ToString();
        StddevSampleListTextBox.Text = valuesList.SampleStandardDeviation().ToString();
        StddevPopulationArrayTextBox.Text = _valuesArray.PopulationStandardDeviation().ToString();
        StddevPopulationListTextBox.Text = valuesList.PopulationStandardDeviation().ToString();

        await RedrawHistogram(_valuesArray);
    }

    private async Task RedrawHistogram(int[] valuesArray)
    {
        if (valuesArray is null)
        {
            return;
        }

        Rectangle[] rectangles = new Rectangle[]
        {
            ColumnRectangle2,
            ColumnRectangle3,
            ColumnRectangle4,
            ColumnRectangle5,
            ColumnRectangle6,
            ColumnRectangle7,
            ColumnRectangle8,
            ColumnRectangle9,
            ColumnRectangle10,
            ColumnRectangle11,
            ColumnRectangle12,
        };

        Dictionary<int, int> values = new();
        foreach (int value in valuesArray)
        {
            if (values.ContainsKey(value))
            {
                values[value]++;
            }
            else
            {
                values.Add(value, 1);
            }
        }
        int totalHeight = BottomPixel() - TopPixel() - 50;
        double unit = (double)totalHeight / values.Max(kvp => kvp.Value);
        bool allZero = true;
        for (int i = 0; i < rectangles.Length; i++)
        {
            if (!values.ContainsKey(i + 2))
            {
                values[i + 2] = 0;
            }

            rectangles[i].Height = unit * values[i + 2];
            if (rectangles[i].Height > 0)
            {
                allZero = false;
            }
        }

        if (allZero)
        {
            ContentDialog dialog = new()
            {
                // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
                XamlRoot = Content.XamlRoot,
                Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                Title = "Warning!",
                CloseButtonText = "Ok",
                DefaultButton = ContentDialogButton.Close,
                Content = "Too many values to show in histogram"
            };
            _ = await dialog.ShowAsync();
        }
    }

    private int TopPixel()
    {
        // Get the TextBox object
        TextBox myTextBox = NumberOfValuesTextBox;

        // Get the position of the TextBox within its parent container
        GeneralTransform transform = myTextBox.TransformToVisual((UIElement)myTextBox.Parent);
        Point position = transform.TransformPoint(new Point(0, 0));

        // Calculate the pixel value of the bottommost part of the TextBox
        double topPixelValue = position.Y;
        return topPixelValue > int.MaxValue ? throw new Exception("Top pixel value too large")
                                            : (int)topPixelValue;
    }

    private int BottomPixel()
    {
        // Get the TextBox object
        TextBlock myTextBlock = HistogramLabel2;

        // Get the ActualHeight of the TextBox
        double textBlockHeight = myTextBlock.ActualHeight;

        // Get the position of the TextBox within its parent container
        GeneralTransform transform = myTextBlock.TransformToVisual((UIElement)myTextBlock.Parent);
        Point position = transform.TransformPoint(new Point(0, 0));

        // Calculate the pixel value of the bottommost part of the TextBox
        double bottomPixelValue = position.Y + textBlockHeight;
        return bottomPixelValue > int.MaxValue ? throw new Exception("Bottom pixel value too large")
                                               : (int)bottomPixelValue;
    }

    //    private int GetWindowHeight()
    //    {
    //        // Get the current window handle
    //        IntPtr hWnd = WindowNative.GetWindowHandle(this);

    //        // Get the window ID
    //        WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);

    //        // Get the AppWindow object
    //        AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

    //        // Get the current size of the window
    //        Windows.Graphics.SizeInt32 currentSize = appWindow.Size;

    //        // The current height of the window is stored in the Height property
    //        return currentSize.Height;
    //    }
}
