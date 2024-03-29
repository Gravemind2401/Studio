﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Sandbox.Utilities
{
    public class GridLengthConverter : IValueConverter
    {
        public GridUnitType GridUnitType { get; set; } = GridUnitType.Pixel;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => new GridLength((double)value, GridUnitType);
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => ((GridLength)value).Value;
    }
}
