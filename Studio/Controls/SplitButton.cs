using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Studio.Controls
{
    //This is intended only to be used within toolbars.
    //The dropdown items must be defined as part of the ContextMenu property.
    public class SplitButton : Button
    {
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(SplitButton), new PropertyMetadata(false));

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == ContextMenuProperty)
            {
                if (e.OldValue is ContextMenu oldMenu)
                {
                    oldMenu.PlacementTarget = null;
                    BindingOperations.ClearBinding(oldMenu, ContextMenu.IsOpenProperty);
                }

                if (e.NewValue is ContextMenu newMenu)
                {
                    newMenu.PlacementTarget = this;
                    newMenu.Placement = PlacementMode.Bottom;
                    newMenu.HorizontalOffset = 1;
                    newMenu.VerticalOffset = -1;
                    BindingOperations.SetBinding(newMenu, ContextMenu.IsOpenProperty, new Binding(IsDropDownOpenProperty.Name) { Source = this });
                }
            }
        }
    }
}
