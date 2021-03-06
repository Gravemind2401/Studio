﻿using Prism.Mvvm;
using Studio.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sandbox.Models
{
    public class TabModel : BindableBase
    {
        private ModelBase parent;
        public ModelBase Parent
        {
            get { return parent; }
            set { SetProperty(ref parent, value); }
        }

        private TabItemType usage;
        public TabItemType Usage
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

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set { SetProperty(ref isActive, value); }
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

        private double width;
        public double Width
        {
            get { return width; }
            set { SetProperty(ref width, value); }
        }

        private double height;
        public double Height
        {
            get { return height; }
            set { SetProperty(ref height, value); }
        }
    }
}
