using Prism.Mvvm;
using Sandbox.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.ViewModels
{
    public class WindowViewModel : ModelBase
    {
        private ObservableCollection<TabModel> leftDockItems = new ObservableCollection<TabModel>();
        public ObservableCollection<TabModel> LeftDockItems
        {
            get { return leftDockItems; }
            set { SetProperty(ref leftDockItems, value); }
        }

        private ObservableCollection<TabModel> topDockItems = new ObservableCollection<TabModel>();
        public ObservableCollection<TabModel> TopDockItems
        {
            get { return topDockItems; }
            set { SetProperty(ref topDockItems, value); }
        }

        private ObservableCollection<TabModel> rightDockItems = new ObservableCollection<TabModel>();
        public ObservableCollection<TabModel> RightDockItems
        {
            get { return rightDockItems; }
            set { SetProperty(ref rightDockItems, value); }
        }

        private ObservableCollection<TabModel> bottomDockItems = new ObservableCollection<TabModel>();
        public ObservableCollection<TabModel> BottomDockItems
        {
            get { return bottomDockItems; }
            set { SetProperty(ref bottomDockItems, value); }
        }

        private SplitViewModel content;
        public SplitViewModel Content
        {
            get { return content; }
            set { SetProperty(ref content, value, OnContentChanged); }
        }

        private void OnContentChanged(SplitViewModel prev, SplitViewModel next)
        {
            prev?.SetParent(null, null);
            next?.SetParent(null, this);
        }
    }
}
