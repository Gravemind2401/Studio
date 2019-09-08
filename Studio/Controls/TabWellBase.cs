using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Studio.Controls
{
    public abstract class TabWellBase : TabControl
    {
        private const int MinDetachThreshold = 0;
        private const int MaxDetachThreshold = 30;

        private int DetachThreshold = MinDetachThreshold;
        private int SwapThreshold = 0;

        private static readonly DependencyPropertyKey IsActivePropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(IsActive), typeof(bool), typeof(TabWellBase), new PropertyMetadata(true));

        public static readonly DependencyProperty IsActiveProperty = IsActivePropertyKey.DependencyProperty;

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            internal set { SetValue(IsActivePropertyKey, value); }
        }

        protected TabWellBase() : base()
        {
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

        private void CloseTabCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var tab = e.Parameter as TabWellItem;
            var item = tab == null ? e.Parameter : ItemContainerGenerator.ItemFromContainer(tab);

            if (!Items.Contains(item))
                return;

            var disposable = ((item as ContentControl)?.Content as IDisposable) ?? (item as IDisposable);
            disposable?.Dispose();

            Items.Remove(item);
        }
    }
}
