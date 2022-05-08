using Studio.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Studio.Controls
{
    public abstract class TabWellBase : TabControl, IDockReceiver
    {
        private const int minFloatThreshold = 0;
        private const int maxFloatThreshold = 30;

        public static readonly DependencyProperty FloatTabCommandProperty =
            DependencyProperty.Register(nameof(FloatTabCommand), typeof(ICommand), typeof(TabWellBase), new PropertyMetadata((ICommand)null));

        public static readonly DependencyProperty FloatAllCommandProperty =
            DependencyProperty.Register(nameof(FloatAllCommand), typeof(ICommand), typeof(TabWellBase), new PropertyMetadata((ICommand)null));

        public static readonly DependencyProperty DockCommandProperty =
            DependencyProperty.Register(nameof(DockCommand), typeof(ICommand), typeof(TabWellBase), new PropertyMetadata((ICommand)null));

        public ICommand FloatTabCommand
        {
            get => (ICommand)GetValue(FloatTabCommandProperty);
            set => SetValue(FloatTabCommandProperty, value);
        }

        public ICommand FloatAllCommand
        {
            get => (ICommand)GetValue(FloatAllCommandProperty);
            set => SetValue(FloatAllCommandProperty, value);
        }

        public ICommand DockCommand
        {
            get => (ICommand)GetValue(DockCommandProperty);
            set => SetValue(DockCommandProperty, value);
        }

        internal TabWellItem FirstContainer
        {
            get
            {
                return Enumerable.Range(0, Items.Count)
                    .Select(i => (TabWellItem)ItemContainerGenerator.ContainerFromIndex(i))
                    .OrderByDescending(t => DockManager.GetIsPinned(t))
                    .FirstOrDefault();
            }
        }

        private int floatThreshold = minFloatThreshold;
        private int swapThreshold = 0;

        protected TabWellBase()
        {
            CommandBindings.Add(new CommandBinding(Commands.TabMouseDownCommand, TabMouseDownCommandExecuted));
            CommandBindings.Add(new CommandBinding(Commands.TabMouseMoveCommand, TabMouseMoveCommandExecuted));
            CommandBindings.Add(new CommandBinding(Commands.TabMouseUpCommand, TabMouseUpCommandExecuted));
            CommandBindings.Add(new CommandBinding(Commands.TabLostMouseCaptureCommand, TabLostMouseCaptureCommandExecuted));
            CommandBindings.Add(new CommandBinding(Commands.CloseTabCommand, CloseTabCommandExecuted));

            Loaded += TabWellBase_Loaded;
            Unloaded += TabWellBase_Unloaded;
        }

        private void TabWellBase_Loaded(object sender, RoutedEventArgs e) => DockManager.Register(this);
        private void TabWellBase_Unloaded(object sender, RoutedEventArgs e) => DockManager.Unregister(this);

        protected override DependencyObject GetContainerForItemOverride() => new TabWellItem();
        protected override bool IsItemItsOwnContainerOverride(object item) => item is TabWellItem;

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
                swapThreshold = 0;
                tab.CaptureMouse();
            }

            e.Handled = true;
        }

        //internal command - OriginalSource is TabWellItem, Parameter is MouseButtonEventArgs
        private void TabMouseUpCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var tab = (TabWellItem)e.OriginalSource;
            var mouseArgs = (MouseButtonEventArgs)e.Parameter;

            if (mouseArgs.ChangedButton != MouseButton.Left || !tab.IsMouseCaptured)
                return;

            swapThreshold = 0;
            tab.ReleaseMouseCapture();
            e.Handled = true;
        }

        //internal command - OriginalSource is TabWellItem, Parameter is MouseEventArgs
        private void TabMouseMoveCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var tab = (TabWellItem)e.OriginalSource;
            var mouseArgs = (MouseEventArgs)e.Parameter;

            if (!tab.IsMouseCaptured || !tab.IsLoaded)
                return;

            var item = ItemContainerGenerator.ItemFromContainer(tab);
            var index = Items.IndexOf(item);
            var pos = mouseArgs.GetPosition(tab);

            var offset = mouseArgs.GetPosition(this);
            var floatBounds = new Rect(
                pos.X - offset.X,
                -floatThreshold,
                ActualWidth,
                tab.ActualHeight + floatThreshold * 2
            );

            if (!floatBounds.Contains(pos))
            {
                var args = new FloatEventArgs(this, tab, mouseArgs);
                tab.ReleaseMouseCapture();
                DockManager.SetIsPinned(tab, false);
                FloatTabCommand?.TryExecute(args);
                return;
            }

            if (pos.X > 0 && pos.X < tab.ActualWidth)
                swapThreshold = 0;

            var isPinned = DockManager.GetIsPinned(tab);
            var grouped = Items.OfType<object>()
                .Select(i => ItemContainerGenerator.ContainerFromItem(i))
                .Where(c => DockManager.GetIsPinned(c) == isPinned);

            var collection = ItemsSource as IList ?? Items;
            if (pos.X < -swapThreshold && index > 0)
            {
                if (grouped.TakeWhile(c => c != tab).LastOrDefault() is FrameworkElement prevTab)
                {
                    swapThreshold = (int)Math.Ceiling(Math.Max(0, prevTab.ActualWidth - tab.ActualWidth));
                    floatThreshold = maxFloatThreshold;

                    collection.Remove(item);
                    collection.Insert(index - 1, item);

                    tab = (TabWellItem)ItemContainerGenerator.ContainerFromIndex(index - 1);
                }
            }
            else if (pos.X > tab.ActualWidth + swapThreshold && index < Items.Count - 1)
            {
                if (grouped.SkipWhile(c => c != tab).Skip(1).FirstOrDefault() is FrameworkElement nextTab)
                {
                    swapThreshold = (int)Math.Ceiling(Math.Max(0, nextTab.ActualWidth - tab.ActualWidth));
                    floatThreshold = maxFloatThreshold;

                    collection.Remove(item);
                    collection.Insert(index + 1, item);

                    tab = (TabWellItem)ItemContainerGenerator.ContainerFromIndex(index + 1);
                }
            }

            SelectedItem = item;
            tab.CaptureMouse();
        }

        //internal command - OriginalSource is TabWellItem, Parameter is MouseEventArgs
        private void TabLostMouseCaptureCommandExecuted(object sender, ExecutedRoutedEventArgs e) => floatThreshold = minFloatThreshold;
        #endregion
    }
}
