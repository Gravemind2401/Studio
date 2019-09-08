using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Studio.Controls
{
    public class DocumentWell : TabWellBase
    {
        private int pinnedCount = 0;

        public DocumentWell() : base()
        {
            CommandBindings.Add(new CommandBinding(Commands.PinTabCommand, PinTabCommandExecuted));
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
    }
}
