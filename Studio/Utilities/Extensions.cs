﻿using Studio.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Studio.Utilities
{
    internal static class Extensions
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

        internal static object GetContainerContext(this FrameworkElement element)
        {
            var lineage = new List<DependencyObject> { element };
            var parent = VisualTreeHelper.GetParent(element);
            while (parent != null)
            {
                lineage.Insert(0, parent);
                if (parent is ItemsControl)
                    break;
                else parent = VisualTreeHelper.GetParent(parent);
            }

            var ic = lineage[0] as ItemsControl;
            if (ic != null)
            {
                lineage.Reverse();
                foreach (var d in lineage)
                {
                    var item = ic.ItemContainerGenerator.ItemFromContainer(d);
                    if (item != DependencyProperty.UnsetValue)
                        return item;
                }
            }

            return element.DataContext;
        }

        public static DpiScale GetDpi(this Visual visual)
        {
            var method = typeof(Visual).GetMethod(nameof(GetDpi), System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            return (DpiScale)method.Invoke(visual, null);
        }

        public static Point PointToScreenScaled(this Visual visual, Point point)
        {
            var scale = visual.GetDpi();
            point = visual.PointToScreen(point);
            return new Point(point.X * 1d / scale.DpiScaleX, point.Y * 1d / scale.DpiScaleY);
        }

        public static Orientation? GetDockOrientation(this DockTarget d)
        {
            if (d == DockTarget.Center)
                return null;
            else if (d == DockTarget.DockLeft || d == DockTarget.DockRight || d == DockTarget.SplitLeft || d == DockTarget.SplitRight)
                return Orientation.Horizontal;
            else return Orientation.Vertical;
        }

        public static Dock? GetDockSide(this DockTarget d)
        {
            if (d == DockTarget.Center)
                return null;
            else if (d == DockTarget.DockLeft || d == DockTarget.SplitLeft)
                return Dock.Left;
            else if (d == DockTarget.DockTop || d == DockTarget.SplitTop)
                return Dock.Top;
            else if (d == DockTarget.DockRight || d == DockTarget.SplitRight)
                return Dock.Right;
            else return Dock.Bottom;
        }
    }
}
