using System;
using System.Globalization;

namespace WorkItems.Converter;

internal sealed class InverseBoolConverter : ValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is not bool boolValue || !boolValue;
    }
}

internal sealed class StringToBoolConverter : ValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string str && !string.IsNullOrWhiteSpace(str);
    }
}

internal sealed class StringToObjectConverter : ValueConverter
{
    public object EmptyObject { get; set; }
    public object NonEmptyObject { get; set; }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string stringValue && !string.IsNullOrWhiteSpace(stringValue) ? NonEmptyObject : EmptyObject;
    }
}

internal sealed class BoolToObjectConverter : ValueConverter
{
    public object TrueObject { get; set; }
    public object FalseObject { get; set; }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue && boolValue ? TrueObject : FalseObject;
    }
}

internal sealed class ObjectToBoolConverter : ValueConverter
{
    public bool NullBool { get; set; } = false;
    public bool NonNullBool { get; set; } = true;

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null ? NonNullBool : NullBool;
    }
}

internal sealed class ObjectToObjectConverter : ValueConverter
{
    public object NullObject { get; set; }
    public object NonNullObject { get; set; }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null ? NonNullObject : NullObject;
    }
}

internal sealed class CompareConverter : ValueConverter
{
    public object TrueValue { get; set; }
    public object FalseValue { get; set; }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return object.Equals(value, parameter) ? TrueValue : FalseValue;
    }
}
