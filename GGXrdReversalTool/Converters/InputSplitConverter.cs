using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace GGXrdReversalTool.Converters;

[ValueConversion(typeof(string),typeof(IEnumerable<string>))]
public class InputSplitConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        var input = value?.ToString();

        if (input == null)
        {
            return Enumerable.Empty<string>();
        }
        
        return input.ToUpper()
                .Where(c => c != '!')
                .Where(c => c is '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9' or 'P' or 'K' or 'S' or 'H' or 'D')
                .Select(c=> c.ToString())
            ;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}