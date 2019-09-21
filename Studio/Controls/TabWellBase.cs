using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Studio.Utilities;
using System.Collections;

namespace Studio.Controls
{
    public abstract class TabWellBase : TabControl
    {
        private const int MinDetachThreshold = 0;
        private const int MaxDetachThreshold = 30;

        private int DetachThreshold = MinDetachThreshold;
        private int SwapThreshold = 0;

        protected TabWellBase()
        {
            CommandBindings.Add(new CommandBinding(Commands.TabMouseDownCommand, TabMouseDownCommandExecuted));
            CommandBindings.Add(new CommandBinding(Commands.TabMouseMoveCommand, TabMouseMoveCommandExecuted));
            CommandBindings.Add(new CommandBinding(Commands.TabMouseUpCommand, TabMouseUpCommandExecuted));
            CommandBindings.Add(new CommandBinding(Commands.TabLostMouseCaptureCommand, TabLostMouseCaptureCommandExecuted));
            CommandBindings.Add(new CommandBinding(Commands.CloseTabCommand, CloseTabCommandExecuted));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TabWellItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TabWellItem;
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            DockManager.SetIsActive(this, true);
            base.OnPreviewMouseDown(e);
        }

        #region Command Handlers
        private void CloseTabCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var item = ItemContainerGenerator.ItemFromContainer((e.OriginalSource as FrameworkElement)?.FindVisualAncestor<TabItem>());

            if (!Items.Contains(item))
                return;

            var disposable = ((item as ContentControl)?.Content as IDisposable) ?? (item as IDisposable);
            disposable?.Dispose();

            Items.Remove(item);
        }

        //internal command - OriginalSource is TabWellItem, Parameter is MouseButtonEventArgs
        private void TabMouseDownCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var tab = (TabWellItem)e.OriginalSource;
            var mouseArgs = (MouseButtonEventArgs)e.Parameter;

            if (mouseArgs.ChangedButton == MouseButton.Left)
            {
                SwapThreshold = 0;
                tab.CaptureMouse();
            }

            e.Handled = true;
        }

        //internal command - OriginalSource is TabWellItem, Parameter is MouseButtonEventArgs
        private void TabMouseUpCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var tab = (TabWellItem)e.OriginalSource;
            var mouseArgs = (MouseButtonEventArgs)e.Parameter;

            if (mouseArgs.ChangedButton != MouseButton.Left || !tab.IsMouseCaptured) return;

            SwapThreshold = 0;
            tab.ReleaseMouseCapture();
            e.Handled = true;
        }

        //internal command - OriginalSource is TabWellItem, Parameter is MouseEventArgs
        private void TabMouseMoveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var tab = (TabWellItem)e.OriginalSource;
            var mouseArgs = (MouseEventArgs)e.Parameter;

            if (!tab.IsMouseCaptured || !tab.IsLoaded) return;

            var item = ItemContainerGenerator.ItemFromContainer(tab);
            var index = Items.IndexOf(item);
            var pos = mouseArgs.GetPosition(tab);

            var offset = mouseArgs.GetPosition(this);
            var detachBounds = new Rect(
                pos.X - offset.X,
                -DetachThreshold,
                this.ActualWidth,
                tab.ActualHeight + DetachThreshold * 2
            );

            if (!detachBounds.Contains(pos))
            {
                tab.ReleaseMouseCapture();
                DocumentTabPanel.SetIsPinned(tab, false);
                System.Diagnostics.Debugger.Break();
                return;
            }

            if (pos.X > 0 && pos.X < tab.ActualWidth) SwapThreshold = 0;

            var isPinned = DocumentTabPanel.GetIsPinned(tab);
            var grouped = Items.OfType<object>()
                .Select(i => ItemContainerGenerator.ContainerFromItem(i))
                .Where(c => DocumentTabPanel.GetIsPinned(c) == isPinned);

            var collection = ItemsSource as IList ?? Items as IList;
            if (pos.X < -SwapThreshold && index > 0)
            {
                var prevTab = grouped.TakeWhile(c => c != tab).LastOrDefault() as FrameworkElement;
                if (prevTab != null)
                {
                    SwapThreshold = (int)Math.Ceiling(Math.Max(0, prevTab.ActualWidth - tab.ActualWidth));
                    DetachThreshold = MaxDetachThreshold;

                    collection.Remove(item);
                    collection.Insert(index - 1, item);

                    tab = (TabWellItem)ItemContainerGenerator.ContainerFromIndex(index - 1);
                }
            }
            else if (pos.X > tab.ActualWidth + SwapThreshold && index < Items.Count - 1)
            {
                var nextTab = grouped.SkipWhile(c => c != tab).Skip(1).FirstOrDefault() as FrameworkElement;
                if (nextTab != null)
                {
                    SwapThreshold = (int)Math.Ceiling(Math.Max(0, nextTab.ActualWidth - tab.ActualWidth));
                    DetachThreshold = MaxDetachThreshold;

                    collection.Remove(item);
                    collection.Insert(index + 1, item);

                    tab = (TabWellItem)ItemContainerGenerator.ContainerFromIndex(index + 1);
                }
            }

            SelectedItem = item;
            tab.CaptureMouse();
        }

        //internal command - OriginalSource is TabWellItem, Parameter is MouseEventArgs
        private void TabLostMouseCaptureCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            DetachThreshold = MinDetachThreshold;
        }
        #endregion
    }
}
