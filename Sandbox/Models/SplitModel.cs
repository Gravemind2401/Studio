using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sandbox.Models
{
    public class SplitModel : BindableBase
    {
        private object item1;
        public object Item1
        {
            get { return item1; }
            set { SetProperty(ref item1, value); }
        }

        private object item2;
        public object Item2
        {
            get { return item2; }
            set { SetProperty(ref item2, value); }
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

        public SplitModel()
        {
            Item1Size = new GridLength(1, GridUnitType.Star);
            Item2Size = new GridLength(1, GridUnitType.Star);
        }
    }
}
