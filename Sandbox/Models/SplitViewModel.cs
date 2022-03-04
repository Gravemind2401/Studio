using Sandbox.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Sandbox.Models
{
    public class SplitViewModel : ModelBase
    {
        private readonly ObservableCollection<ModelBase> items = new ObservableCollection<ModelBase>();
        public ReadOnlyObservableCollection<ModelBase> Items { get; }

        private Orientation orientation = Orientation.Horizontal;
        public Orientation Orientation
        {
            get => orientation;
            set => SetProperty(ref orientation, value);
        }

        public ModelBase Item1
        {
            get => Items[0];
            set
            {
                if (Item1 == value)
                    return;

                var prev = Item1;
                items[0] = value;

                RaisePropertyChanged();
                OnItemChanged(prev, value);
            }
        }

        public ModelBase Item2
        {
            get => Items[1];
            set
            {
                if (Item2 == value)
                    return;

                var prev = Item2;
                items[1] = value;

                RaisePropertyChanged();
                OnItemChanged(prev, value);
            }
        }

        public GridLength? Item1Size
        {
            get => Items[0]?.PanelSize;
            set
            {
                if (Item1 != null)
                    Item1.PanelSize = value.Value;
            }
        }

        public GridLength? Item2Size
        {
            get => Items[1]?.PanelSize;
            set
            {
                if (Item2 != null)
                    Item2.PanelSize = value.Value;
            }
        }

        public SplitViewModel()
        {
            items.Add(null);
            items.Add(null);
            Items = new ReadOnlyObservableCollection<ModelBase>(items);
        }

        public SplitViewModel(Dock dock, TabWellModelBase content) : this()
        {
            Orientation = dock == Dock.Left || dock == Dock.Right ? Orientation.Horizontal : Orientation.Vertical;

            if (dock == Dock.Left || dock == Dock.Top)
            {
                Item1 = content;
                Item1.PanelSize = new GridLength(dock == Dock.Left ? content.Width : content.Height);
                Item2 = null;
            }
            else
            {
                Item1 = null;
                Item2 = content;
                Item2.PanelSize = new GridLength(dock == Dock.Right ? content.Width : content.Height);
            }
        }

        private void OnItemChanged(ModelBase prev, ModelBase next)
        {
            prev?.SetParent(null, null);
            next?.SetParent(this, ParentViewModel);

            if (next == null)
            {
                var remaining = Item1 ?? Item2;
                if (remaining == null) return;
                Item1 = Item2 = null;

                if (ParentModel != null)
                    ParentModel.Replace(this, remaining);
                else if (ParentViewModel != null)
                    ParentViewModel.Content = remaining;
            }
        }

        public bool Remove(ModelBase item)
        {
            if (Item1 == item)
            {
                Item1 = null;
                return true;
            }
            else if (Item2 == item)
            {
                Item2 = null;
                return true;
            }

            return false;
        }

        public bool Replace(ModelBase prev, ModelBase next)
        {
            if (Item1 == prev)
            {
                var sizeTemp = Item1Size;
                Item1 = next;
                Item1Size = sizeTemp;
                return true;
            }
            else if (Item2 == prev)
            {
                var sizeTemp = Item2Size;
                Item2 = next;
                Item2Size = sizeTemp;
                return true;
            }

            return false;
        }

        public bool Add(ModelBase item)
        {
            if (Item1 == null)
            {
                Item1 = item;
                return true;
            }
            else if (Item2 == null)
            {
                Item2 = item;
                return true;
            }

            return false;
        }

        internal override void SetParent(SplitViewModel parentModel, WindowViewModel parentViewModel)
        {
            base.SetParent(parentModel, parentViewModel);

            Item1?.SetParent(this, parentViewModel);
            Item2?.SetParent(this, parentViewModel);
        }

        internal override IEnumerable<TabModel> AllTabs
        {
            get
            {
                var tabs1 = Item1?.AllTabs ?? Enumerable.Empty<TabModel>();
                var tabs2 = Item2?.AllTabs ?? Enumerable.Empty<TabModel>();
                return tabs1.Concat(tabs2);
            }
        }
    }
}
