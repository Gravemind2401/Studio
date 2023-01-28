using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Studio.Controls
{
    [TemplatePart(Name = nameof(PART_TitleBar), Type = typeof(FrameworkElement))]
    public class ToolWell : TabWellBase
    {
        private const string PART_TitleBar = "PART_TitleBar";

        private FrameworkElement titleBar;

        public static object ToolTabStyleKey { get; } = new Guid("6bed4f5f-4da7-4c65-8efa-a5b7b999885a");

        public static readonly DependencyProperty TogglePinStatusCommandProperty =
            DependencyProperty.Register(nameof(TogglePinStatusCommand), typeof(ICommand), typeof(ToolWell), new PropertyMetadata((ICommand)null));

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register(nameof(CloseCommand), typeof(ICommand), typeof(ToolWell), new PropertyMetadata(Commands.CloseToolCommand));

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register(nameof(Caption), typeof(object), typeof(ToolWell), new PropertyMetadata((object)null));

        public static readonly DependencyProperty HideTitleBarProperty =
            DependencyProperty.Register(nameof(HideTitleBar), typeof(bool), typeof(ToolWell), new PropertyMetadata(false));

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

        public object Caption
        {
            get => GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }

        public bool HideTitleBar
        {
            get => (bool)GetValue(HideTitleBarProperty);
            set => SetValue(HideTitleBarProperty, value);
        }

        static ToolWell()
        {
            TabStripPlacementProperty.OverrideMetadata(typeof(ToolWell), new FrameworkPropertyMetadata(Dock.Bottom));
        }

        public ToolWell()
        {
            CommandBindings.Add(new CommandBinding(Commands.CloseToolCommand, CloseToolCommandExecuted));
        }

        private void CloseToolCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            (ItemsSource as IList ?? Items)?.Remove(SelectedItem);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (titleBar != null)
            {
                titleBar.MouseDown -= CaptionPanel_MouseDown;
                titleBar.MouseUp -= CaptionPanel_MouseUp;
                titleBar.MouseMove -= CaptionPanel_MouseMove;
            }

            titleBar = Template.FindName(PART_TitleBar, this) as FrameworkElement;

            if (titleBar != null)
            {
                titleBar.MouseDown += CaptionPanel_MouseDown;
                titleBar.MouseUp += CaptionPanel_MouseUp;
                titleBar.MouseMove += CaptionPanel_MouseMove;
            }
        }

        private Point dragStart;
        private const int floatThreshold = 10;

        private void CaptionPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left || e.Source is Button)
                return;

            dragStart = e.GetPosition(this);
            titleBar.CaptureMouse();
            e.Handled = true;
        }

        private void CaptionPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left || !titleBar.IsMouseCaptured)
                return;

            titleBar.ReleaseMouseCapture();
        }

        private void CaptionPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!titleBar.IsMouseCaptured)
                return;

            var pos = e.GetPosition(titleBar);
            var dist = Math.Sqrt(Math.Pow(pos.X - dragStart.X, 2) + Math.Pow(pos.Y - dragStart.Y, 2));

            if (dist > floatThreshold)
            {
                var args = new FloatEventArgs(this, null, e);
                titleBar.ReleaseMouseCapture();
                FloatAllCommand?.TryExecute(args);
            }
        }
    }
}
