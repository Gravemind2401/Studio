using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Studio.Controls
{
    public enum TabItemType
    {
        Document,
        Tool
    }

    public class TabWellItem : TabItem
    {
        public static readonly DependencyProperty ItemTypeProperty =
            DependencyProperty.Register(nameof(ItemType), typeof(TabItemType), typeof(TabWellItem), new PropertyMetadata(TabItemType.Document));

        public static readonly DependencyProperty TogglePinStatusCommandProperty =
            DependencyProperty.Register(nameof(TogglePinStatusCommand), typeof(ICommand), typeof(TabWellItem), new PropertyMetadata(Commands.PinTabCommand));

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register(nameof(CloseCommand), typeof(ICommand), typeof(TabWellItem), new PropertyMetadata(Commands.CloseTabCommand));

        public TabItemType ItemType
        {
            get => (TabItemType)GetValue(ItemTypeProperty);
            set => SetValue(ItemTypeProperty, value);
        }

        public ICommand TogglePinStatusCommand
        {
            get => (ICommand)GetValue(TogglePinStatusCommandProperty);
            set => SetValue(TogglePinStatusCommandProperty, value);
        }

        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        public TabWellItem()
        {
            Loaded += TabWellItem_Loaded;
            Unloaded += TabWellItem_Unloaded;
        }

        private void TabWellItem_Loaded(object sender, RoutedEventArgs e) => DockManager.Register(this);
        private void TabWellItem_Unloaded(object sender, RoutedEventArgs e) => DockManager.Unregister(this);

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            base.OnMouseDown(e);

            var bounds = new Rect(0, 0, ActualWidth, ActualHeight); //make sure event source is the header element not the content area
            if (e.Handled || !bounds.Contains(e.GetPosition(this)))
                return;

            if (e.ChangedButton == MouseButton.Middle)
            {
                if (e.ButtonState == MouseButtonState.Pressed)
                    CloseCommand?.TryExecute(DataContext ?? this, this);
            }
            else
                Commands.TabMouseDownCommand.TryExecute(e, this);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            base.OnMouseMove(e);

            if (!e.Handled)
                Commands.TabMouseMoveCommand.TryExecute(e, this);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            base.OnMouseUp(e);

            if (!e.Handled)
                Commands.TabMouseUpCommand.TryExecute(e, this);
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            base.OnLostMouseCapture(e);

            if (!e.Handled)
                Commands.TabLostMouseCaptureCommand.TryExecute(e, this);
        }
    }
}
