using Sandbox.Controls;
using Sandbox.ViewModels;
using Studio.Controls;
using System.Windows;
using System.Windows.Controls;

namespace Sandbox.Models
{
    public class DocumentWellModel : TabWellModelBase
    {
        private DocContainerModel parent;
        public DocContainerModel Parent
        {
            get => parent;
            set => SetProperty(ref parent, value);
        }

        protected override void TogglePinStatusExecuted(TabModel item) => item.IsPinned = !item.IsPinned;

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
                InnerDock(e);
            else
                OuterDock(e);
        }

        private void InnerDock(DockEventArgs e)
        {
            var index = Parent.Children.IndexOf(this);

            if (e.TargetDock == DockTarget.SplitRight || e.TargetDock == DockTarget.SplitBottom)
                index++;

            var orientation = e.TargetDock == DockTarget.SplitLeft || e.TargetDock == DockTarget.SplitRight
                ? Orientation.Horizontal
                : Orientation.Vertical;

            var groups = e.SourceContent.OfType<TabWellModelBase>().ToList();
            var newGroup = new DocumentWellModel();

            foreach (var group in groups)
            {
                foreach (var item in group.Children.ToList())
                {
                    group.Children.Remove(item);
                    item.IsPinned = item.IsActive = false;
                    newGroup.Children.Add(item);
                }
            }

            Parent.Orientation = orientation;
            Parent.Children.Insert(index, newGroup);

            foreach (var child in Parent.Children)
                child.PanelSize = new GridLength(1, GridUnitType.Star);

            e.SourceWindow.Close();
        }

        private void OuterDock(DockEventArgs e)
        {
            var groups = e.SourceContent.OfType<TabWellModelBase>().ToList();
            var newGroup = new ToolWellModel() { Dock = (Dock)((int)e.TargetDock - 5) };

            foreach (var group in groups)
            {
                foreach (var item in group.Children.ToList())
                {
                    group.Children.Remove(item);
                    item.IsPinned = item.IsActive = false;
                    newGroup.Children.Add(item);
                }
            }

            var newSplit = new SplitViewModel();
            newSplit.Orientation = e.TargetDock == DockTarget.DockLeft || e.TargetDock == DockTarget.DockRight
                ? Orientation.Horizontal
                : Orientation.Vertical;

            if (ParentModel == null)
                ParentViewModel.Content = newSplit;
            else
                ParentModel.Replace(Parent, newSplit);

            if (e.TargetDock == DockTarget.DockTop || e.TargetDock == DockTarget.DockLeft)
            {
                newSplit.Item1 = newGroup;
                newSplit.Item2 = Parent;
                newSplit.Item1Size = new GridLength(e.DesiredSize);
            }
            else
            {
                newSplit.Item1 = Parent;
                newSplit.Item2 = newGroup;
                newSplit.Item2Size = new GridLength(e.DesiredSize);
            }

            newGroup.IsActive = true;
            newGroup.SelectedItem = newGroup.Children.First();

            e.SourceWindow.Close();
        }

        protected override void OnChildrenChanged()
        {
            if (Children.Count == 0)
                Parent?.Children.Remove(this);
        }
    }
}
