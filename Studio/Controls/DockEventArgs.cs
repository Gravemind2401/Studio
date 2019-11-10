using Studio.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Studio.Controls
{
    public class DockEventArgs : EventArgs
    {
        public Window SourceWindow { get; }

        public IEnumerable<object> SourceContent { get; }

        public double DesiredSize { get; }

        public DockTarget TargetDock { get; }

        public object TargetItem { get; }

        internal DockEventArgs(IEnumerable<FrameworkElement> sourceItems, FrameworkElement targetElement, DockTarget targetDock, object targetIndex)
        {
            SourceWindow = Window.GetWindow(sourceItems.First());
            SourceContent = sourceItems.Select(i => i.GetContainerContext() ?? i);
            TargetDock = targetDock;
            TargetItem = targetIndex;

            if (targetDock.GetDockOrientation() == Orientation.Horizontal)
                DesiredSize = Math.Min(SourceWindow.ActualWidth, targetElement.ActualWidth / 2);
            else
                DesiredSize = Math.Min(SourceWindow.ActualHeight, targetElement.ActualHeight / 2);
        }
    }
}
