using Prism.Mvvm;
using Studio.Controls;
using System.Windows;

namespace Sandbox.Models
{
    public class TabModel : BindableBase
    {
        private ModelBase parent;
        public ModelBase Parent
        {
            get => parent;
            set => SetProperty(ref parent, value);
        }

        private TabItemType usage;
        public TabItemType Usage
        {
            get => usage;
            set => SetProperty(ref usage, value);
        }

        private bool isPinned;
        public bool IsPinned
        {
            get => isPinned;
            set => SetProperty(ref isPinned, value);
        }

        private bool isActive;
        public bool IsActive
        {
            get => isActive;
            set => SetProperty(ref isActive, value);
        }

        private string header;
        public string Header
        {
            get => header;
            set => SetProperty(ref header, value);
        }

        private string toolTip;
        public string ToolTip
        {
            get => toolTip;
            set => SetProperty(ref toolTip, value);
        }

        private UIElement content;
        public UIElement Content
        {
            get => content;
            set => SetProperty(ref content, value);
        }

        private double width;
        public double Width
        {
            get => width;
            set => SetProperty(ref width, value);
        }

        private double height;
        public double Height
        {
            get => height;
            set => SetProperty(ref height, value);
        }
    }
}
