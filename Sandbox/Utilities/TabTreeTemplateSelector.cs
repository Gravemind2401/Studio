using Sandbox.Models;
using Studio.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sandbox.Utilities
{
    public class TabTreeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            if (item is SplitViewModel)
                return element.FindResource("SplitPaneTemplate") as DataTemplate;
            else if (item is DocContainerModel)
                return element.FindResource("DocumentContainerTemplate") as DataTemplate;
            else if (item is ToolWellModel)
                return element.FindResource("ToolGroupTemplate") as DataTemplate;
            else
                return base.SelectTemplate(item, container);
        }
    }
}
