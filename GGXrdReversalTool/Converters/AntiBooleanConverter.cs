using System;
using System.Globalization;
using System.Windows.Data;

namespace GGXrdReversalTool.Converters;

[ValueConversion(typeof(bool),typeof(bool))]
public class AntiBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool valueBool)
        {
            return !valueBool;
        }
        
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}