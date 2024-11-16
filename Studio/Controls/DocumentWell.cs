using Studio.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Studio.Controls
{
    [StyleTypedProperty(Property = nameof(OverflowItemStyle), StyleTargetType = typeof(MenuItem))]
    public class DocumentWell : TabWellBase
    {
        public static object DocumentTabStyleKey { get; } = new Guid("98887b53-8f42-4300-a9ff-75168d55f2b3");

        public static readonly DependencyProperty OverflowItemStyleProperty =
            DependencyProperty.Register(nameof(OverflowItemStyle), typeof(Style), typeof(DocumentWell), new PropertyMetadata(null));

        public static readonly DependencyProperty OverflowItemSelectedCommandProperty =
            DependencyProperty.Register(nameof(OverflowItemSelectedCommand), typeof(ICommand), typeof(DocumentWell), new PropertyMetadata(Commands.SelectTabCommand));

        public static readonly DependencyProperty OverflowItemClosedCommandProperty =
            DependencyProperty.Register(nameof(OverflowItemClosedCommand), typeof(ICommand), typeof(DocumentWell), new PropertyMetadata(Commands.CloseTabCommand));

        public static readonly DependencyProperty PinOnSeparateRowProperty =
            DependencyProperty.Register(nameof(PinOnSeparateRow), typeof(bool), typeof(DocumentWell), new PropertyMetadata(false));

        public Style OverflowItemStyle
        {
            get => (Style)GetValue(OverflowItemStyleProperty);
            set => SetValue(OverflowItemStyleProperty, value);
        }

        public ICommand OverflowItemSelectedCommand
        {
            get => (ICommand)GetValue(OverflowItemSelectedCommandProperty);
            set => SetValue(OverflowItemSelectedCommandProperty, value);
        }

        public ICommand OverflowItemClosedCommand
        {
            get => (ICommand)GetValue(OverflowItemClosedCommandProperty);
            set => SetValue(OverflowItemClosedCommandProperty, value);
        }

        public bool PinOnSeparateRow
        {
            get => (bool)GetValue(PinOnSeparateRowProperty);
            set => SetValue(PinOnSeparateRowProperty, value);
        }

        public DocumentWell() : base()
        {
            CommandBindings.Add(Commands.PinTabCommand, PinTabCommandExecuted);
            CommandBindings.Add(Commands.SelectTabCommand, SelectTabCommandExecuted);
        }

        private void PinTabCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var container = (e.OriginalSource as FrameworkElement)?.FindVisualAncestor<TabItem>();
            DockManager.SetIsPinned(container, !DockManager.GetIsPinned(container));
        }

        private void SelectTabCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var item = ItemContainerGenerator.ItemFromContainer((e.OriginalSource as FrameworkElement)?.FindVisualAncestor<TabItem>());
            if (Items.Contains(item))
                SelectedItem = item;
        }
    }
}
