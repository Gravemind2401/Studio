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
        private int pinnedCount = 0;

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
            var item = e.Parameter as DependencyObject;
            if (item == null || !Items.Contains(item)) return;

            var selected = SelectedItem;
            var isPinned = !DocumentTabPanel.GetIsPinned(item);
            DocumentTabPanel.SetIsPinned(item, isPinned);

            if (!isPinned) pinnedCount--;

            Items.Remove(item);
            Items.Insert(pinnedCount, item);

            if (isPinned) pinnedCount++;

            SelectedItem = selected;
        }

        private void SelectTabCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (Items.Contains(e.Parameter))
                SelectedItem = e.Parameter;
        }
    }
}
