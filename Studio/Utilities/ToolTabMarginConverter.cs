﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Studio.Utilities
{
    public class ToolTabMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(Thickness))
                return new Thickness();

            var thickness = (Thickness)value;
            return new Thickness(0, -thickness.Bottom, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
