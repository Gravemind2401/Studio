using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Sandbox.ViewModels;
using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace Sandbox.Models
{
    public class SplitViewModel : ModelBase
    {
        public ObservableCollection<ItemWrapper> Items { get; }

        private Orientation orientation = Orientation.Horizontal;
        public Orientation Orientation
        {
            get { return orientation; }
            set { SetProperty(ref orientation, value); }
        }

        public ModelBase Item1
        {
            get { return Items.Count < 1 ? null : Items[0]?.Content; }
            set
            {
                if (Item1 == value)
                    return;

                var prev = Item1;

                if (Items.Count < 1)
                    Items.Add(new ItemWrapper(value));
                else Items[0] = new ItemWrapper(value);

                RaisePropertyChanged();
                OnItemChanged(prev, value);
            }
        }

        public ModelBase Item2
        {
            get { return Items.Count < 2 ? null : Items[1]?.Content; }
            set
            {
                if (Item2 == value)
                    return;

                var prev = Item2;

                if (Items.Count < 2)
                    Items.Add(new ItemWrapper(value));
                else Items[1] = new ItemWrapper(value);

                RaisePropertyChanged();
                OnItemChanged(prev, value);
            }
        }

        public GridLength? Item1Size
        {
            get { return Items.Count < 1 ? (GridLength?)null : Items[0].DesiredSize; }
            set
            {
                if (Items.Count >= 1)
                    Items[0].DesiredSize = value.Value;
            }
        }

        public GridLength? Item2Size
        {
            get { return Items.Count < 2 ? (GridLength?)null : Items[1].DesiredSize; }
            set
            {
                if (Items.Count >= 2)
                    Items[1].DesiredSize = value.Value;
            }
        }

        public SplitViewModel()
        {
            Items = new ObservableCollection<ItemWrapper>();
        }

        public SplitViewModel(Dock dock, TabGroupModel content) : this()
        {
            Orientation = dock == Dock.Left || dock == Dock.Right ? Orientation.Horizontal : Orientation.Vertical;

            if (dock == Dock.Left || dock == Dock.Top)
            {
                Items.Add(new ItemWrapper(content));
                Items[0].DesiredSize = new GridLength(dock == Dock.Left ? content.Width : content.Height);
                Items.Add(null);
            }
            else
            {
                Items.Add(null);
                Items.Add(new ItemWrapper(content));
                Items[1].DesiredSize = new GridLength(dock == Dock.Right ? content.Width : content.Height);
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
                Items.Clear();

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

        public sealed class ItemWrapper : BindableBase
        {
            public double MinSize { get; }
            public ModelBase Content { get; }

            private GridLength desiredSize;
            public GridLength DesiredSize
            {
                get { return desiredSize; }
                set { SetProperty(ref desiredSize, value); }
            }

            internal ItemWrapper(ModelBase content)
            {
                Content = content;
                MinSize = 65d;

                desiredSize = new GridLength(1, GridUnitType.Star);
            }
        }
    }
}
