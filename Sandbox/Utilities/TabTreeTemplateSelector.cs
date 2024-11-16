using Sandbox.Models;
using System.Windows;
using System.Windows.Controls;

namespace Sandbox.Utilities
{
    public class TabTreeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            return item switch
            {
                SplitViewModel => element.FindResource("SplitPaneTemplate") as DataTemplate,
                DocContainerModel => element.FindResource("DocumentContainerTemplate") as DataTemplate,
                ToolWellModel => element.FindResource("ToolGroupTemplate") as DataTemplate,
                _ => base.SelectTemplate(item, container)
            };
        }
    }
}
