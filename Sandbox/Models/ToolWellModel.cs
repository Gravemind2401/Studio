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
    public class ToolWellModel : TabWellModelBase
    {
        private bool isWindow;
        public bool IsWindow
        {
            get { return isWindow; }
            internal set { SetProperty(ref isWindow, value); }
        }

        private Dock dock;
        public Dock Dock
        {
            get { return dock; }
            internal set { SetProperty(ref dock, value); }
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

        internal override IEnumerable<TabModel> AllTabs => Children;
    }
}
