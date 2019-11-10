using Prism.Commands;
using Sandbox.Controls;
using Sandbox.ViewModels;
using Studio.Controls;
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
    public class DocumentWellModel : TabWellModelBase
    {
        private DocContainerModel parent;
        public DocContainerModel Parent
        {
            get { return parent; }
            set { SetProperty(ref parent, value); }
        }

        protected override void TogglePinStatusExecuted(TabModel item)
        {
            item.IsPinned = !item.IsPinned;
        }

        protected override void FloatTabExecuted(FloatEventArgs e)
        {
            var item = e.DataContext as TabModel;
            Children.Remove(item);

            var model = new WindowViewModel { IsRafted = true };
            var group = new DocumentWellModel();
            group.Children.Add(item);
            model.Content = new DocContainerModel(group);

            var wnd = new RaftedWindow
            {
                Model = model,
                Left = e.VisualBounds.X,
                Top = e.VisualBounds.Y,
                Width = e.VisualBounds.Width,
                Height = e.VisualBounds.Height
            };

            model.Host = wnd;

            if (ParentViewModel != null && ParentViewModel.IsRafted && !ParentViewModel.AllTabs.Any())
                ShowOnClose(ParentViewModel.Host, wnd);
            else
            {
                wnd.Show();
                wnd.DragMove();
            }
        }

        protected override void DockExecuted(DockEventArgs e)
        {
            if (e.TargetDock == DockTarget.Center)
                base.DockExecuted(e);
            else if (e.TargetDock == DockTarget.SplitLeft || e.TargetDock == DockTarget.SplitTop || e.TargetDock == DockTarget.SplitRight || e.TargetDock == DockTarget.SplitBottom)
                SplitDock(e);
            else OuterDock(e);
        }

        private void SplitDock(DockEventArgs e)
        {

        }

        private void OuterDock(DockEventArgs e)
        {

        }

        protected override void OnChildrenChanged()
        {
            if (Children.Count == 0)
                Parent?.Children.Remove(this);
        }
    }
}
