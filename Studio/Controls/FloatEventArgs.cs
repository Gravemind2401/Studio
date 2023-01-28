using Studio.Utilities;
using System.Windows;
using System.Windows.Input;

namespace Studio.Controls
{
    public class FloatEventArgs : EventArgs
    {
        public Window Source { get; }
        public object DataContext { get; }
        public Point MouseOffset { get; }
        public double? TabOffset { get; }
        public Rect VisualBounds { get; }

        internal FloatEventArgs(TabWellBase tabWell, TabWellItem tab, MouseEventArgs e)
        {
            Source = Window.GetWindow(tabWell);
            DataContext = tab?.GetContainerContext() ?? tabWell.GetContainerContext();
            MouseOffset = e.GetPosition(tabWell);

            if (tab != null)
                TabOffset = tab.TranslatePoint(new Point(0, 0), tabWell).X;

            var loc = tabWell.PointToScreenScaled(new Point(0, 0));
            VisualBounds = new Rect(loc.X, loc.Y, tabWell.ActualWidth, tabWell.ActualHeight);
        }
    }
}
