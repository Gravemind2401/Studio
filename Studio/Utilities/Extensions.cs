using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Studio.Utilities
{
    public static class Extensions
    {
        public static T FindLogicalAncestor<T>(this FrameworkElement element) where T : DependencyObject
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            while (element.Parent != null)
            {
                if (element.Parent is T)
                    return element.Parent as T;
                else if (element.Parent is FrameworkElement)
                    element = (FrameworkElement)element.Parent;
                else break;
            }

            return default(T);
        }

        public static T FindVisualAncestor<T>(this DependencyObject element) where T : DependencyObject
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            element = VisualTreeHelper.GetParent(element);
            while (element != null)
            {
                if (element is T)
                    return element as T;
                else if (element is DependencyObject)
                    element = VisualTreeHelper.GetParent(element);
                else break;
            }

            return default(T);
        }
    }
}
