using Prism.Commands;
using Prism.Mvvm;
using Sandbox.Controls;
using Sandbox.Models;
using Studio.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sandbox.ViewModels
{
    public class WindowViewModel : ModelBase
    {
        public const double DefaultDockSize = 260d;

        private bool isRafted;
        public bool IsRafted
        {
            get { return isRafted; }
            internal set { SetProperty(ref isRafted, value); }
        }

        internal Window Host { get; set; }

        private ObservableCollection<TabModel> leftDockItems;
        public ObservableCollection<TabModel> LeftDockItems
        {
            get { return leftDockItems; }
            set { SetProperty(ref leftDockItems, value, OnCollectionChanged); }
        }

        private ObservableCollection<TabModel> topDockItems;
        public ObservableCollection<TabModel> TopDockItems
        {
            get { return topDockItems; }
            set { SetProperty(ref topDockItems, value, OnCollectionChanged); }
        }

        private ObservableCollection<TabModel> rightDockItems;
        public ObservableCollection<TabModel> RightDockItems
        {
            get { return rightDockItems; }
            set { SetProperty(ref rightDockItems, value, OnCollectionChanged); }
        }

        private ObservableCollection<TabModel> bottomDockItems;
        public ObservableCollection<TabModel> BottomDockItems
        {
            get { return bottomDockItems; }
            set { SetProperty(ref bottomDockItems, value, OnCollectionChanged); }
        }

        private TabModel selectedDockItem;
        public TabModel SelectedDockItem
        {
            get { return selectedDockItem; }
            set { SetProperty(ref selectedDockItem, value); }
        }

        private ModelBase content;
        public ModelBase Content
        {
            get { return content; }
            set { SetProperty(ref content, value, OnContentChanged); }
        }

        public DelegateCommand<TabModel> CloseTabCommand { get; }
        public DelegateCommand<TabModel> TogglePinStatusCommand { get; }
        public DelegateCommand<DockEventArgs> DockCommand { get; }

        public WindowViewModel()
        {
            CloseTabCommand = new DelegateCommand<TabModel>(CloseTabExecuted);
            TogglePinStatusCommand = new DelegateCommand<TabModel>(TogglePinStatusExecuted);
            DockCommand = new DelegateCommand<DockEventArgs>(DockExecuted);

            LeftDockItems = new ObservableCollection<TabModel>();
            TopDockItems = new ObservableCollection<TabModel>();
            RightDockItems = new ObservableCollection<TabModel>();
            BottomDockItems = new ObservableCollection<TabModel>();
        }

        private void CloseTabExecuted(TabModel item)
        {
            LeftDockItems?.Remove(SelectedDockItem);
            TopDockItems?.Remove(SelectedDockItem);
            RightDockItems?.Remove(SelectedDockItem);
            BottomDockItems?.Remove(SelectedDockItem);
        }

        private void TogglePinStatusExecuted(TabModel item)
        {
            item = SelectedDockItem;
            if (LeftDockItems.Contains(item))
            {
                LeftDockItems.Remove(item);
                AddItem(item, null, Dock.Left);
            }
            else if (TopDockItems.Contains(item))
            {
                TopDockItems.Remove(item);
                AddItem(item, null, Dock.Top);
            }
            else if (RightDockItems.Contains(item))
            {
                RightDockItems.Remove(item);
                AddItem(item, null, Dock.Right);
            }
            else if (BottomDockItems.Contains(item))
            {
                BottomDockItems.Remove(item);
                AddItem(item, null, Dock.Bottom);
            }
        }

        private void DockExecuted(DockEventArgs e)
        {
            var groups = e.SourceContent.OfType<TabWellModelBase>().ToList();
            var newGroup = new ToolWellModel() { Dock = (Dock)((int)e.TargetDock - 5) };

            foreach (var group in groups)
            {
                var allChildren = group.Children.ToList();
                foreach (var item in allChildren)
                {
                    group.Children.Remove(item);
                    item.IsPinned = false;
                    item.IsActive = false;

                    newGroup.Children.Add(item);
                }
            }

            var newSplit = new SplitViewModel();
            newSplit.Orientation = e.TargetDock == DockTarget.DockLeft || e.TargetDock == DockTarget.DockRight
                ? Orientation.Horizontal
                : Orientation.Vertical;

            var existing = Content;
            Content = null;

            if (e.TargetDock == DockTarget.DockTop || e.TargetDock == DockTarget.DockLeft)
            {
                newSplit.Item1 = newGroup;
                newSplit.Item2 = existing;
                newSplit.Item1Size = new GridLength(DefaultDockSize);
            }
            else
            {
                newSplit.Item1 = existing;
                newSplit.Item2 = newGroup;
                newSplit.Item2Size = new GridLength(DefaultDockSize);
            }

            Content = newSplit;
            newGroup.IsActive = true;
            newGroup.SelectedItem = newGroup.Children.First();

            e.SourceWindow.Close();
        }

        public void AddItem(TabModel item, ModelBase target, Dock dock)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            var group = new ToolWellModel() { Width = item.Width, Height = item.Height };
            group.Children.Add(item);

            var container = new SplitViewModel(dock, group);

            if (target == null)
            {
                target = Content;
                Content = container;
            }
            else if (target.ParentModel != null)
                target.ParentModel.Replace(target, container);
            else
            {
                System.Diagnostics.Debugger.Break();
                throw new InvalidOperationException();
            }

            container.Add(target);
        }

        private void OnContentChanged(ModelBase prev, ModelBase next)
        {
            prev?.SetParent(null, null);
            next?.SetParent(null, this);
        }

        private void OnCollectionChanged(ObservableCollection<TabModel> prev, ObservableCollection<TabModel> next)
        {
            Unsubscribe(prev);
            Subscribe(next);
        }

        private void Unsubscribe(ObservableCollection<TabModel> collection)
        {
            if (collection == null)
                return;

            foreach (var tool in collection)
                tool.Parent = null;

            collection.CollectionChanged -= DockItems_CollectionChanged;
        }

        private void Subscribe(ObservableCollection<TabModel> collection)
        {
            if (collection == null)
                return;

            foreach (var tool in collection)
                tool.Parent = this;

            collection.CollectionChanged += DockItems_CollectionChanged;
        }

        private void DockItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var tool in e.OldItems.OfType<TabModel>())
                    tool.Parent = null;
            }

            if (e.NewItems != null)
            {
                foreach (var tool in e.NewItems.OfType<TabModel>())
                    tool.Parent = this;
            }
        }

        internal override IEnumerable<TabModel> AllTabs => Content?.AllTabs ?? Enumerable.Empty<TabModel>();
    }
}
