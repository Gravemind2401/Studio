using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Studio.Utilities
{
    public class ThicknessConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return new Thickness();

            double left, top, right, bottom;
            left = top = right = bottom = 0;

            if (values.Length > 0)
                left = top = right = bottom = values[0] as double? ?? 0;
            if (values.Length > 1)
                top = bottom = values[1] as double? ?? 0;
            if (values.Length > 2)
                right = values[2] as double? ?? 0;
            if (values.Length > 3)
                bottom = values[3] as double? ?? 0;

            return new Thickness(left, top, right, bottom);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
