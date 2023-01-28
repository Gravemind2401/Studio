using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Studio.Utilities
{
    public class TreeViewItemMarginConverter : IMultiValueConverter
    {
        public double IndentWidth { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return new Thickness();

            if (!(values[0] is TreeViewItem && values[1] is Thickness))
                return new Thickness();

            var item = (TreeViewItem)values[0];
            var margin = (Thickness)values[1];

            int depth = 0;
            while ((item = GetParent(item)) != null)
                depth++;

            margin.Left += depth * IndentWidth;
            return margin;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();

        private static TreeViewItem GetParent(TreeViewItem item)
        {
            var parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView) && parent != null)
                parent = VisualTreeHelper.GetParent(parent);
            return parent as TreeViewItem;
        }
    }
}
