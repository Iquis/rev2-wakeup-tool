using System;
using System.Globalization;
using System.Windows.Data;

namespace GGXrdWakeupDPUtil.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class HitCounterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int input = (int)value;

            return $"{input} hit{(input > 1 ? "s" : "")}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
