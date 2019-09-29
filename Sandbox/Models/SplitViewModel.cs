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

        public SplitViewModel()
        {
            Item1Size = new GridLength(1, GridUnitType.Star);
            Item2Size = new GridLength(1, GridUnitType.Star);
        }

        private void OnItemChanged(ModelBase prev, ModelBase next)
        {
            prev?.SetParent(null, null);
            next?.SetParent(this, ParentViewModel);

            if (ParentModel != null && next == null)
            {
                var remaining = Item1 ?? Item2;
                if (remaining == null) return;

                Item1 = Item2 = null;
                ParentModel.Replace(this, remaining);
            }
        }

        public void Remove(ModelBase item)
        {
            if (Item1 == item)
                Item1 = null;
            else if (Item2 == item)
                Item2 = null;
        }

        public void Replace(ModelBase prev, ModelBase next)
        {
            if (Item1 == prev)
                Item1 = next;
            else if (Item2 == prev)
                Item2 = next;
        }

        internal override void SetParent(SplitViewModel parentModel, WindowViewModel parentViewModel)
        {
            base.SetParent(parentModel, parentViewModel);

            Item1?.SetParent(this, parentViewModel);
            Item2?.SetParent(this, parentViewModel);
        }
    }
}
