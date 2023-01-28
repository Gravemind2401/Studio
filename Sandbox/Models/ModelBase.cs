using Prism.Mvvm;
using Sandbox.ViewModels;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Sandbox.Models
{
    public class ModelBase : BindableBase
    {
        private SplitViewModel parentModel;
        public SplitViewModel ParentModel
        {
            get => parentModel;
            private set => SetProperty(ref parentModel, value);
        }

        private WindowViewModel parentViewModel;
        public WindowViewModel ParentViewModel
        {
            get => parentViewModel;
            private set => SetProperty(ref parentViewModel, value);
        }

        private GridLength panelSize = new GridLength(1, GridUnitType.Star);
        public GridLength PanelSize
        {
            get => panelSize;
            set => SetProperty(ref panelSize, value);
        }

        private double minPanelSize = 65d;
        public double MinPanelSize
        {
            get => minPanelSize;
            set => SetProperty(ref minPanelSize, value);
        }

        protected bool SetProperty<T>(ref T storage, T value, Action<T, T> onChanged, [CallerMemberName] string propertyName = null)
        {
            var prev = storage;
            if (SetProperty(ref storage, value, propertyName))
            {
                onChanged(prev, value);
                return true;
            }
            else return false;
        }

        protected void ShowOnClose(Window close, Window show)
        {
            //if we try to close window A then show and drag window B we get the error 'Can only call DragMove when primary mouse button is down.'
            //this is a workaround to ensure window A has closed before windo B attempts to show and drag
            close.Closed += (s, e) =>
            {
                show.Show();
                show.DragMove();
            };
            close.Close();
        }

        internal virtual void SetParent(SplitViewModel parentModel, WindowViewModel parentViewModel)
        {
            ParentModel = parentModel;
            ParentViewModel = parentViewModel;
        }

        internal virtual IEnumerable<TabModel> AllTabs
        {
            get
            {
                yield break;
            }
        }
    }
}
