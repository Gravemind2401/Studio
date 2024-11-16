using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Studio.Controls
{
    //This is intended only to be used within toolbars.
    //The dropdown items must be defined as part of the ContextMenu property.

    [TemplatePart(Name = PART_MenuButton, Type = typeof(FrameworkElement))]
    public class SplitButton : Button
    {
        private const string PART_MenuButton = "PART_MenuButton";

        private FrameworkElement MenuButton;
        private bool IsMenuClick;

        #region Dependency Properties
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(SplitButton), new PropertyMetadata(false));

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }
        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            MenuButton = Template.FindName(PART_MenuButton, this) as FrameworkElement;
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

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            IsMenuClick = MenuButton != null && VisualTreeHelper.HitTest(MenuButton, e.GetPosition(MenuButton)) != null;
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnClick()
        {
            if (IsMenuClick)
                IsDropDownOpen = true;
            else
                base.OnClick();
        }
    }
}
