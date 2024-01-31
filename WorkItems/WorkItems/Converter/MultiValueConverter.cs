using Microsoft.Maui.Controls;
using System;
using System.Globalization;

namespace WorkItems.Converter;

/// <summary>
/// Base class helper for multi-value converters
/// </summary>
public abstract class MultiValueConverter : IMultiValueConverter
{
    public virtual object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }

    public virtual object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new InvalidOperationException();
    }

    object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return Convert(values, targetType, parameter, culture);
    }

    object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return ConvertBack(value, targetTypes, parameter, culture);
    }
}