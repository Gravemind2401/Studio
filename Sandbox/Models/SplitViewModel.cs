using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

        private object item1;
        public object Item1
        {
            get { return item1; }
            set { SetProperty(ref item1, value, OnItemChanged); }
        }

        private object item2;
        public object Item2
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

        public SplitViewModel()
        {
            Item1Size = new GridLength(1, GridUnitType.Star);
            Item2Size = new GridLength(1, GridUnitType.Star);
        }

        private void OnItemChanged(object prev, object next)
        {
            (prev as ModelBase)?.SetParent(null, null);
            (next as ModelBase)?.SetParent(this, ParentViewModel);

            if (ParentModel != null && next == null)
            {
                var remaining = Item1 ?? Item2;
                if (remaining == null) return;

                Item1 = Item2 = null;
                ParentModel.Replace(this, remaining);
            }
        }

        public void Remove(object item)
        {
            if (Item1 == item)
                Item1 = null;
            else if (Item2 == item)
                Item2 = null;
        }

        public void Replace(object prev, object next)
        {
            if (Item1 == prev)
                Item1 = next;
            else if (Item2 == prev)
                Item2 = next;
        }
    }
}
