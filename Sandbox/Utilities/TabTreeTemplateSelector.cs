using Sandbox.Models;
using System;
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
            
            return base.SelectTemplate(item, container);
        }
    }
}
