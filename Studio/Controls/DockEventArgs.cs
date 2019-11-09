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
        public Window SourceWindow { get; }

        public IEnumerable<object> SourceContent { get; }

        public DockTarget TargetDock { get; }

        public object TargetItem { get; }

        internal DockEventArgs(Window source, IEnumerable<object> sourceContent, DockTarget targetDock, object targetIndex)
        {
            SourceWindow = source;
            SourceContent = sourceContent;
            TargetDock = targetDock;
            TargetItem = targetIndex;
        }
    }
}
