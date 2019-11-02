using Prism.Commands;
using Studio.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Sandbox.Models
{
    public class TabGroupModel : ModelBase
    {
        private ObservableCollection<TabModel> children = new ObservableCollection<TabModel>();
        public ObservableCollection<TabModel> Children
        {
            get { return children; }
            set { SetProperty(ref children, value, OnCollectionChanged); }
        }

        private readonly TabUsage groupType;
        public TabUsage GroupType => groupType;

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set { SetProperty(ref isActive, value); }
        }

        private bool isWindow;
        public bool IsWindow
        {
            get { return isWindow; }
            internal set { SetProperty(ref isWindow, value); }
        }

        private TabModel selectedItem;
        public TabModel SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value); }
        }

        private double width;
        public double Width
        {
            get { return width; }
            set { SetProperty(ref width, value, UpdateChildrenWidth); }
        }

        private double height;
        public double Height
        {
            get { return height; }
            set { SetProperty(ref height, value, UpdateChildrenHeight); }
        }

        private Dock dock;
        public Dock Dock
        {
            get { return dock; }
            internal set { SetProperty(ref dock, value); }
        }

        public DelegateCommand<TabModel> CloseTabCommand { get; }
        public DelegateCommand<TabModel> TogglePinStatusCommand { get; }
        public DelegateCommand<TabModel> SelectItemCommand { get; }
        public DelegateCommand<DockEventArgs> DockCommand { get; }

        public TabGroupModel(TabUsage groupType)
        {
            this.groupType = groupType;

            CloseTabCommand = new DelegateCommand<TabModel>(CloseTabExecuted);
            TogglePinStatusCommand = new DelegateCommand<TabModel>(TogglePinStatusExecuted);
            SelectItemCommand = new DelegateCommand<TabModel>(SelectItemExecuted);
            DockCommand = new DelegateCommand<DockEventArgs>(DockExecuted);
            Subscribe(children);
        }

        private void CloseTabExecuted(TabModel item)
        {
            Children.Remove(item ?? SelectedItem);
        }

        private void TogglePinStatusExecuted(TabModel item)
        {
            if (GroupType == TabUsage.Document)
                item.IsPinned = !item.IsPinned;
            else
            {
                var temp = ParentViewModel;
                foreach (var c in Children.ToList())
                {
                    children.Remove(c);
                    c.IsActive = true;

                    if (Dock == Dock.Left)
                        temp.LeftDockItems.Add(c);
                    else if (Dock == Dock.Top)
                        temp.TopDockItems.Add(c);
                    else if (Dock == Dock.Right)
                        temp.RightDockItems.Add(c);
                    else if (Dock == Dock.Bottom)
                        temp.BottomDockItems.Add(c);
                }
            }
        }

        private void SelectItemExecuted(TabModel item)
        {
            SelectedItem = item;
        }

        private void DockExecuted(DockEventArgs e)
        {
            var groups = e.SourceContent.OfType<TabGroupModel>().ToList();
            var target = e.TargetIndex as TabModel;
            var index = target == null || target.IsPinned ? 0 : Children.IndexOf(target);

            foreach (var group in groups)
            {
                var allChildren = group.Children.ToList();
                foreach (var item in allChildren)
                {
                    group.Children.Remove(item);
                    item.IsPinned = false;
                    item.IsActive = false;

                    Children.Insert(index, item);
                }
            }

            e.Source.Close();
            IsActive = true;
            SelectedItem = Children[index];
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

            collection.CollectionChanged -= Children_CollectionChanged;
        }

        private void Subscribe(ObservableCollection<TabModel> collection)
        {
            if (collection == null)
                return;

            foreach (var tool in collection)
                tool.Parent = this;

            collection.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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

            if (GroupType == TabUsage.Tool && Children.Count == 0)
                Remove();
        }

        public void Remove()
        {
            if (ParentModel != null)
                ParentModel.Remove(this);
            else if (ParentViewModel != null)
                ParentViewModel.Content = null;
        }

        private void UpdateChildrenWidth()
        {
            foreach (var item in Children)
                item.Width = Width;
        }

        private void UpdateChildrenHeight()
        {
            foreach (var item in Children)
                item.Height = Height;
        }
    }
}
