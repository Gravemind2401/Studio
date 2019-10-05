using Studio.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Studio.Controls
{
    public class DockManager
    {
        #region DockGroup
        public static readonly DependencyProperty DockGroupProperty =
            DependencyProperty.RegisterAttached("DockGroup", typeof(string), typeof(DockManager), new PropertyMetadata(string.Empty), ValidateDockGroup);

        public static string GetDockGroup(DependencyObject obj)
        {
            return (string)obj.GetValue(DockGroupProperty);
        }

        public static void SetDockGroup(DependencyObject obj, string value)
        {
            obj.SetValue(DockGroupProperty, value);
        }

        public static bool ValidateDockGroup(object value)
        {
            return value as string != null;
        }
        #endregion

        #region IsActive
        private static readonly Dictionary<string, DependencyObject> activeObjects = new Dictionary<string, DependencyObject>();

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.RegisterAttached("IsActive", typeof(bool), typeof(DockManager), new PropertyMetadata(false, IsActiveChanged));

        public static bool GetIsActive(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsActiveProperty);
        }

        public static void SetIsActive(DependencyObject obj, bool value)
        {
            obj.SetValue(IsActiveProperty, value);
        }

        public static void IsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var group = GetDockGroup(d);
            if (!(bool)e.NewValue)
            {
                activeObjects.Remove(group);
                return;
            }

            if (activeObjects.ContainsKey(group))
                SetIsActive(activeObjects[group], false);
            
            if (!activeObjects.ContainsKey(group))
                activeObjects.Add(group, d);
        }
        #endregion

        #region IsPinned
        public static readonly DependencyProperty IsPinnedProperty =
            DependencyProperty.RegisterAttached("IsPinned", typeof(bool), typeof(DockManager), new PropertyMetadata(false, IsPinnedChanged));

        public static bool GetIsPinned(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPinnedProperty);
        }

        public static void SetIsPinned(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPinnedProperty, value);
        }

        public static void IsPinnedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (VisualTreeHelper.GetParent(d) as UIElement)?.InvalidateVisual();
        } 
        #endregion
    }
}
