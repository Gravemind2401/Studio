using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Studio.Utilities
{
    public class DockToOrientationConverter : IValueConverter
    {
        public Orientation LeftRightValue { get; set; }
        public Orientation TopBottomValue { get; set; }

        public DockToOrientationConverter()
        {
            LeftRightValue = Orientation.Vertical;
            TopBottomValue = Orientation.Horizontal;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dock = value as Dock?;

            if (dock == Dock.Left || dock == Dock.Right)
                return LeftRightValue;
            else if (dock == Dock.Top || dock == Dock.Bottom)
                return TopBottomValue;
            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
