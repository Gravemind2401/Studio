using Studio.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Studio.Controls
{
    public class FloatEventArgs : EventArgs
    {
        public object DataContext { get; }

        public Point MouseOffset { get; }

        public double? TabOffset { get; }

        public Rect VisualBounds { get; }

        internal FloatEventArgs(TabWellBase tabWell, TabWellItem tab, MouseEventArgs e)
        {
            DataContext = tab?.DataContext ?? tabWell.DataContext;
            MouseOffset = e.GetPosition(tabWell);

            if (tab != null)
                TabOffset = tab.TranslatePoint(new Point(0, 0), tabWell).X;

            var loc = tabWell.PointToScreenScaled(new Point(0, 0));
            VisualBounds = new Rect(loc.X, loc.Y, tabWell.ActualWidth, tabWell.ActualHeight);
        }
    }
}
