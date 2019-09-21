using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Sandbox.Models;

namespace Sandbox.Utilities
{
    public class TabTreeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            if (item is SplitModel)
                return element.FindResource("SplitPaneTemplate") as DataTemplate;

            var tabGroupModel = item as TabGroupModel;
            if (tabGroupModel != null)
            {
                var key = tabGroupModel.GroupType == TabUsage.Document
                    ? "DocumentGroupTemplate"
                    : "ToolGroupTemplate";

                return element.FindResource(key) as DataTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
