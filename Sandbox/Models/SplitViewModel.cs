using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Sandbox.ViewModels;

namespace Sandbox.Models
{
    public class SplitViewModel : ModelBase
    {
        private Orientation orientation = Orientation.Horizontal;
        public Orientation Orientation
        {
            get { return orientation; }
            set { SetProperty(ref orientation, value); }
        }

        private ModelBase item1;
        public ModelBase Item1
        {
            get { return item1; }
            set { SetProperty(ref item1, value, OnItemChanged); }
        }

        private ModelBase item2;
        public ModelBase Item2
        {
            get { return item2; }
            set { SetProperty(ref item2, value, OnItemChanged); }
        }

        private GridLength item1Size;
        public GridLength Item1Size
        {
            get { return item1Size; }
            set { SetProperty(ref item1Size, value); }
        }

        private GridLength item2Size;
        public GridLength Item2Size
        {
            get { return item2Size; }
            set { SetProperty(ref item2Size, value); }
        }

        public SplitViewModel(Dock dock, TabGroupModel content)
        {
            Orientation = dock == Dock.Left || dock == Dock.Right ? Orientation.Horizontal : Orientation.Vertical;

            if (dock == Dock.Left || dock == Dock.Top)
            {
                Item1Size = new GridLength(dock == Dock.Left ? content.Width : content.Height);
                Item2Size = new GridLength(1, GridUnitType.Star);
                Item1 = content;
            }
            else
            {
                Item1Size = new GridLength(1, GridUnitType.Star);
                Item2Size = new GridLength(dock == Dock.Right ? content.Width : content.Height);
                Item2 = content;
            }
        }

        public SplitViewModel()
        {
            Item1Size = new GridLength(1, GridUnitType.Star);
            Item2Size = new GridLength(1, GridUnitType.Star);
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
                Item1 = next;
                return true;
            }
            else if (Item2 == prev)
            {
                Item2 = next;
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
    }
}
