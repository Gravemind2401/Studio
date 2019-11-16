using Prism.Commands;
using Sandbox.ViewModels;
using Studio.Controls;
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
        public ObservableCollection<DocumentWellModel> Children { get; }

        private Orientation orientation;
        public Orientation Orientation
        {
            get { return orientation; }
            set { SetProperty(ref orientation, value); }
        }

        public DelegateCommand<DockEventArgs> DockCommand { get; }

        public DocContainerModel()
        {
            DockCommand = new DelegateCommand<DockEventArgs>(DockExecuted);
            Children = new ObservableCollection<DocumentWellModel>();
            Children.CollectionChanged += Children_CollectionChanged;
        }

        public DocContainerModel(DocumentWellModel initial) : this()
        {
            Children.Add(initial);
        }

        private void DockExecuted(DockEventArgs e)
        {
            var groups = e.SourceContent.OfType<TabWellModelBase>().ToList();
            var newGroup = new DocumentWellModel();

            foreach (var group in groups)
            {
                var allChildren = group.Children.ToList();
                foreach (var item in allChildren)
                {
                    group.Children.Remove(item);
                    item.IsPinned = false;
                    item.IsActive = false;

                    newGroup.Children.Add(item);
                }
            }

            e.SourceWindow.Close();
            Children.Add(newGroup);
            newGroup.IsActive = true;
            newGroup.SelectedItem = newGroup.Children[0];
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

            foreach (var child in Children)
                child.SetParent(parentModel, parentViewModel);
        }

        internal override IEnumerable<TabModel> AllTabs => Children.SelectMany(c => c.Children);
    }
}
