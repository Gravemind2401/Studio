using Prism.Mvvm;
using Sandbox.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox.Models
{
    public class ModelBase : BindableBase
    {
        private SplitViewModel parentModel;
        public SplitViewModel ParentModel
        {
            get { return parentModel; }
            internal set { SetProperty(ref parentModel, value); }
        }

        private WindowViewModel parentViewModel;
        public WindowViewModel ParentViewModel
        {
            get { return parentViewModel; }
            internal set { SetProperty(ref parentViewModel, value); }
        }

        protected bool SetProperty<T>(ref T storage, T value, Action<T, T> onChanged, [CallerMemberName]string propertyName = null)
        {
            var prev = storage;
            if (SetProperty(ref storage, value, propertyName))
            {
                onChanged(prev, value);
                return true;
            }
            else return false;
        }

        internal void SetParent(SplitViewModel parentModel, WindowViewModel parentViewModel)
        {
            if ((ParentModel == null && ParentViewModel == null) || (parentModel == null && parentViewModel == null))
            {
                ParentModel = parentModel;
                ParentViewModel = parentViewModel;
            }
            else throw new InvalidOperationException("Item already has a parent!");
        }
    }
}
