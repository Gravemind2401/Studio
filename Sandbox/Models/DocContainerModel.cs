using Sandbox.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Sandbox.Models
{
    public class DocContainerModel : ModelBase
    {
        private ObservableCollection<DocumentWellModel> children = new ObservableCollection<DocumentWellModel>();
        public ObservableCollection<DocumentWellModel> Children
        {
            get { return children; }
            set { SetProperty(ref children, value, OnCollectionChanged); }
        }

        private Orientation orientation;
        public Orientation Orientation
        {
            get { return orientation; }
            set { SetProperty(ref orientation, value); }
        }

        public DocContainerModel()
        {
            Subscribe(Children);
        }

        public DocContainerModel(DocumentWellModel initial) : this()
        {
            Children.Add(initial);
        }

        private void OnCollectionChanged(ObservableCollection<DocumentWellModel> prev, ObservableCollection<DocumentWellModel> next)
        {
            Unsubscribe(prev);
            Subscribe(next);
        }

        private void Unsubscribe(ObservableCollection<DocumentWellModel> collection)
        {
            if (collection == null)
                return;

            foreach (var well in collection)
                well.Parent = null;

            collection.CollectionChanged -= Children_CollectionChanged;
        }

        private void Subscribe(ObservableCollection<DocumentWellModel> collection)
        {
            if (collection == null)
                return;

            foreach (var well in collection)
                well.Parent = this;

            collection.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var well in e.OldItems.OfType<DocumentWellModel>())
                    well.Parent = null;
            }

            if (e.NewItems != null)
            {
                foreach (var well in e.NewItems.OfType<DocumentWellModel>())
                    well.Parent = this;
            }
        }

        internal override void SetParent(SplitViewModel parentModel, WindowViewModel parentViewModel)
        {
            base.SetParent(parentModel, parentViewModel);

            foreach (var child in children)
                child.SetParent(parentModel, parentViewModel);
        }
    }
}
