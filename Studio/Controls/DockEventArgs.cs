using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Studio.Controls
{
    public class DockEventArgs : EventArgs
    {
        public Window Source { get; }

        public IEnumerable<object> SourceContent { get; }

        public DockTarget TargetDock { get; }

        public object TargetIndex { get; }

        internal DockEventArgs(Window source, IEnumerable<object> sourceContent, DockTarget targetDock, object targetIndex)
        {
            Source = source;
            SourceContent = sourceContent;
            TargetDock = targetDock;
            TargetIndex = targetIndex;
        }
    }
}
