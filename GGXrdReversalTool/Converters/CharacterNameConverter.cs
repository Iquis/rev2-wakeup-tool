﻿using System;
using System.Globalization;
using System.Windows.Data;
using GGXrdReversalTool.Library.Domain.Characters;

namespace GGXrdReversalTool.Converters;

[ValueConversion(typeof(CharacterName), typeof(string))]
public class CharacterNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CharacterName characterName)
            return characterName.ToString();

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}