using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GGXrdReversalTool.Library.Models.Inputs;

namespace GGXrdReversalTool.Converters;

[ValueConversion(typeof(string),typeof(IEnumerable<CondensedInput>))]
public class InputConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return Enumerable.Empty<CondensedInput>();
        }
        
        var slotInput = new SlotInput(value.ToString() ?? "");

        return slotInput.CondensedInputList;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}