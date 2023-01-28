using Sandbox.Controls;
using Studio.Controls;
using System.Windows;
using System.Windows.Controls;

namespace Sandbox.Models
{
    public class ToolWellModel : TabWellModelBase
    {
        private bool isWindow;
        public bool IsWindow
        {
            get => isWindow;
            internal set => SetProperty(ref isWindow, value);
        }

        private Dock dock;
        public Dock Dock
        {
            get => dock;
            internal set => SetProperty(ref dock, value);
        }

        protected override void TogglePinStatusExecuted(TabModel item)
        {
            var temp = ParentViewModel;
            foreach (var c in Children.ToList())
            {
                Children.Remove(c);
                c.IsActive = true;

                if (Dock == Dock.Left)
                    temp.LeftDockItems.Add(c);
                else if (Dock == Dock.Top)
                    temp.TopDockItems.Add(c);
                else if (Dock == Dock.Right)
                    temp.RightDockItems.Add(c);
                else if (Dock == Dock.Bottom)
                    temp.BottomDockItems.Add(c);
            }
        }

        protected override void FloatTabExecuted(FloatEventArgs e)
        {
            var item = e.DataContext as TabModel;
            Children.Remove(item);

            var group = new ToolWellModel() { IsWindow = true };
            group.Children.Add(item);

            var wnd = new ToolWindow
            {
                Content = group,
                Left = e.VisualBounds.X,
                Top = e.VisualBounds.Y,
                Width = e.VisualBounds.Width,
                Height = e.VisualBounds.Height
            };

            if (ParentViewModel != null && ParentViewModel.IsRafted && !ParentViewModel.AllTabs.Any())
                ShowOnClose(ParentViewModel.Host, wnd);
            else
            {
                wnd.Show();
                wnd.DragMove();
            }
        }

        protected override void FloatAllExecuted(FloatEventArgs e)
        {
            var pvm = ParentViewModel;

            Remove();
            IsWindow = true;

            var wnd = new ToolWindow
            {
                Content = this,
                Left = e.VisualBounds.X,
                Top = e.VisualBounds.Y,
                Width = e.VisualBounds.Width,
                Height = e.VisualBounds.Height
            };

            if (!pvm.AllTabs.Any())
                ShowOnClose(pvm.Host, wnd);
            else
            {
                wnd.Show();
                wnd.DragMove();
            }
        }

        protected override void DockExecuted(DockEventArgs e)
        {
            if (e.TargetDock == DockTarget.Center)
            {
                base.DockExecuted(e);
                return;
            }

            var groups = e.SourceContent.OfType<TabWellModelBase>().ToList();
            var newGroup = new ToolWellModel() { Dock = Dock };

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

            var newSplit = new SplitViewModel();

            double remainingSize;
            if (e.TargetDock == DockTarget.SplitLeft || e.TargetDock == DockTarget.SplitRight)
            {
                newSplit.Orientation = Orientation.Horizontal;
                remainingSize = Width - e.DesiredSize;
            }
            else
            {
                newSplit.Orientation = Orientation.Vertical;
                remainingSize = Height - e.DesiredSize;
            }

            ParentModel.Replace(this, newSplit);
            if (e.TargetDock == DockTarget.SplitTop || e.TargetDock == DockTarget.SplitLeft)
            {
                newSplit.Item1 = newGroup;
                newSplit.Item2 = this;
                newSplit.Item1Size = new GridLength(e.DesiredSize, GridUnitType.Star);
                newSplit.Item2Size = new GridLength(remainingSize, GridUnitType.Star);
            }
            else
            {
                newSplit.Item1 = this;
                newSplit.Item2 = newGroup;
                newSplit.Item1Size = new GridLength(remainingSize, GridUnitType.Star);
                newSplit.Item2Size = new GridLength(e.DesiredSize, GridUnitType.Star);
            }

            newGroup.IsActive = true;
            newGroup.SelectedItem = newGroup.Children.First();

            e.SourceWindow.Close();
        }

        protected override void OnChildrenChanged()
        {
            if (Children.Count == 0)
                Remove();
        }

        public void Remove()
        {
            if (ParentModel != null)
                ParentModel.Remove(this);
            else if (ParentViewModel != null)
                ParentViewModel.Content = null;
        }
    }
}
