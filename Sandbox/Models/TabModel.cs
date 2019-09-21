using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sandbox.Models
{
    public enum TabUsage
    {
        Document,
        Tool
    }

    public class TabModel : BindableBase
    {
        private TabGroupModel parent;
        public TabGroupModel Parent
        {
            get { return parent; }
            set { SetProperty(ref parent, value); }
        }

        private TabUsage usage;
        public TabUsage Usage
        {
            get { return usage; }
            set { SetProperty(ref usage, value); }
        }

        private bool isPinned;
        public bool IsPinned
        {
            get { return isPinned; }
            set { SetProperty(ref isPinned, value); }
        }

        private string header;
        public string Header
        {
            get { return header; }
            set { SetProperty(ref header, value); }
        }

        private string toolTip;
        public string ToolTip
        {
            get { return toolTip; }
            set { SetProperty(ref toolTip, value); }
        }

        private UIElement content;
        public UIElement Content
        {
            get { return content; }
            set { SetProperty(ref content, value); }
        }
    }
}
