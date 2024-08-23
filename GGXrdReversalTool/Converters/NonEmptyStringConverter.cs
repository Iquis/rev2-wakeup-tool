using System;
using System.Globalization;
using System.Windows.Data;
using GGXrdReversalTool.Library.Domain.Types;

namespace GGXrdReversalTool.Converters;

[ValueConversion(typeof(NonEmptyString), typeof(string))]
public class NonEmptyStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is NonEmptyString characterName)
            return characterName.ToString();

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}