using System;
using System.Globalization;
using System.Windows.Data;

namespace GGXrdWakeupDPUtil.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class AntiBooleanValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool?) value ?? true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool?)value ?? true;
        }
    }
}
