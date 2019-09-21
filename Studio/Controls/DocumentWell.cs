using Studio.Utilities;
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
    [StyleTypedProperty(Property = nameof(OverflowItemStyle), StyleTargetType = typeof(MenuItem))]
    public class DocumentWell : TabWellBase
    {
        private static readonly object tabStyleKey = new Guid("98887b53-8f42-4300-a9ff-75168d55f2b3");
        public static object DocumentTabStyleKey => tabStyleKey;

        public static readonly DependencyProperty OverflowItemStyleProperty =
            DependencyProperty.Register(nameof(OverflowItemStyle), typeof(Style), typeof(DocumentWell), new PropertyMetadata(null));

        public static readonly DependencyProperty OverflowItemSelectedCommandProperty =
            DependencyProperty.Register(nameof(OverflowItemSelectedCommand), typeof(ICommand), typeof(DocumentWell), new PropertyMetadata(Commands.SelectTabCommand));

        public static readonly DependencyProperty PinOnSeparateRowProperty =
            DependencyProperty.Register(nameof(PinOnSeparateRow), typeof(bool), typeof(DocumentWell), new PropertyMetadata(false));

        public Style OverflowItemStyle
        {
            get { return (Style)GetValue(OverflowItemStyleProperty); }
            set { SetValue(OverflowItemStyleProperty, value); }
        }

        public ICommand OverflowItemSelectedCommand
        {
            get { return (ICommand)GetValue(OverflowItemSelectedCommandProperty); }
            set { SetValue(OverflowItemSelectedCommandProperty, value); }
        }

        public bool PinOnSeparateRow
        {
            get { return (bool)GetValue(PinOnSeparateRowProperty); }
            set { SetValue(PinOnSeparateRowProperty, value); }
        }

        public DocumentWell() : base()
        {
            CommandBindings.Add(new CommandBinding(Commands.PinTabCommand, PinTabCommandExecuted));
            CommandBindings.Add(new CommandBinding(Commands.SelectTabCommand, SelectTabCommandExecuted));
        }

        private void PinTabCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var container = (e.OriginalSource as FrameworkElement)?.FindVisualAncestor<TabItem>();
            DocumentTabPanel.SetIsPinned(container, !DocumentTabPanel.GetIsPinned(container));
        }

        private void SelectTabCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var item = ItemContainerGenerator.ItemFromContainer((e.OriginalSource as FrameworkElement)?.FindVisualAncestor<TabItem>());
            if (Items.Contains(item))
                SelectedItem = item;
        }
    }
}
