using Prism.Commands;
using Studio.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Models
{
    public abstract class TabWellModelBase : ModelBase
    {
        private ObservableCollection<TabModel> children = new ObservableCollection<TabModel>();
        public ObservableCollection<TabModel> Children
        {
            get { return children; }
            set { SetProperty(ref children, value, OnCollectionChanged); }
        }

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set { SetProperty(ref isActive, value); }
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

        public DelegateCommand<TabModel> CloseTabCommand { get; }
        public DelegateCommand<TabModel> TogglePinStatusCommand { get; }
        public DelegateCommand<TabModel> SelectItemCommand { get; }
        public DelegateCommand<FloatEventArgs> FloatTabCommand { get; }
        public DelegateCommand<DockEventArgs> DockCommand { get; }

        public TabWellModelBase()
        {
            CloseTabCommand = new DelegateCommand<TabModel>(CloseTabExecuted);
            TogglePinStatusCommand = new DelegateCommand<TabModel>(TogglePinStatusExecuted);
            SelectItemCommand = new DelegateCommand<TabModel>(SelectItemExecuted);
            FloatTabCommand = new DelegateCommand<FloatEventArgs>(FloatTabExecuted);
            DockCommand = new DelegateCommand<DockEventArgs>(DockExecuted);
            Subscribe(children);
        }

        protected virtual void CloseTabExecuted(TabModel item)
        {
            var parent = ParentViewModel;
            Children.Remove(item ?? SelectedItem);

            if (parent != null && parent.IsRafted && !parent.AllTabs.Any())
                parent.Host.Close();
        }

        protected virtual void TogglePinStatusExecuted(TabModel item)
        {

        }

        protected virtual void SelectItemExecuted(TabModel item)
        {
            SelectedItem = item;
        }

        protected virtual void FloatTabExecuted(FloatEventArgs e)
        {

        }

        protected virtual void DockExecuted(DockEventArgs e)
        {
            var groups = e.SourceContent.OfType<TabWellModelBase>().ToList();
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

            foreach (var tab in collection)
                tab.Parent = null;

            collection.CollectionChanged -= Children_CollectionChanged;
        }

        private void Subscribe(ObservableCollection<TabModel> collection)
        {
            if (collection == null)
                return;

            foreach (var tab in collection)
                tab.Parent = this;

            collection.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var tab in e.OldItems.OfType<TabModel>())
                    tab.Parent = null;
            }

            if (e.NewItems != null)
            {
                foreach (var tab in e.NewItems.OfType<TabModel>())
                    tab.Parent = this;
            }

            OnChildrenChanged();
        }

        protected virtual void OnChildrenChanged()
        {

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

        internal override IEnumerable<TabModel> AllTabs => Children;
    }
}
