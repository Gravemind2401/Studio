using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Models
{
    public class TabGroupModel : ModelBase
    {
        private ObservableCollection<TabModel> children = new ObservableCollection<TabModel>();
        public ObservableCollection<TabModel> Children
        {
            get { return children; }
            set
            {
                var prev = children;
                if (SetProperty(ref children, value))
                {
                    Unsubscribe(prev);
                    Subscribe(value);
                }
            }
        }

        private readonly TabUsage groupType;
        public TabUsage GroupType => groupType;

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

        public DelegateCommand<TabModel> CloseTabCommand { get; }
        public DelegateCommand<TabModel> TogglePinStatusCommand { get; }
        public DelegateCommand<TabModel> SelectItemCommand { get; }

        public TabGroupModel(TabUsage groupType)
        {
            this.groupType = groupType;

            CloseTabCommand = new DelegateCommand<TabModel>(CloseTabExecuted);
            TogglePinStatusCommand = new DelegateCommand<TabModel>(TogglePinStatusExecuted);
            SelectItemCommand = new DelegateCommand<TabModel>(SelectItemExecuted);
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
                    temp.LeftDockItems.Add(c);
                }
            }
        }

        private void SelectItemExecuted(TabModel item)
        {
            SelectedItem = item;
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
                ParentModel.Remove(this);
        }
    }
}
