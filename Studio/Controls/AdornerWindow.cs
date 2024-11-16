using System.Windows;

namespace Studio.Controls
{
    internal class AdornerWindow : Window
    {
        private static readonly Dictionary<Window, AdornerWindow> targetLookup = new();

        static AdornerWindow()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(AdornerWindow), new FrameworkPropertyMetadata(typeof(AdornerWindow)));
            AllowsTransparencyProperty.OverrideMetadata(typeof(AdornerWindow), new FrameworkPropertyMetadata(true));
            ShowInTaskbarProperty.OverrideMetadata(typeof(AdornerWindow), new FrameworkPropertyMetadata(false));
            WindowStyleProperty.OverrideMetadata(typeof(AdornerWindow), new FrameworkPropertyMetadata(WindowStyle.None));
            ResizeModeProperty.OverrideMetadata(typeof(AdornerWindow), new FrameworkPropertyMetadata(ResizeMode.NoResize));
            TopmostProperty.OverrideMetadata(typeof(AdornerWindow), new FrameworkPropertyMetadata(true));
        }

        private readonly Window target;
        private DockTargetPanel Panel => Content as DockTargetPanel;

        private AdornerWindow(Window target)
        {
            Background = null;
            this.target = target;

            target.Closed += Target_Closed;
        }

        public static void ForTarget(Window target)
        {
            var wnd = new AdornerWindow(target) { Content = new DockTargetPanel { Visibility = Visibility.Collapsed } };
            targetLookup.Add(target, wnd);
        }

        public static AdornerWindow FromTarget(Window target) => targetLookup[target];

        public new void Show()
        {
            base.Show();

            var pos = target.PointToScreen(new Point());
            Top = pos.Y;
            Left = pos.X;
            Width = target.ActualWidth;
            Height = target.ActualHeight;
        }

        public void SetTargetParams(TargetArgs args)
        {
            Panel.Visibility = Visibility.Visible;
            Panel.AlignToTarget(args);
        }

        public void ClearTargetParams()
        {
            Panel.ClearTarget();
            Panel.Visibility = Visibility.Collapsed;
        }

        private void Target_Closed(object sender, EventArgs e)
        {
            target.Closed -= Target_Closed;
            targetLookup.Remove(target);
            Close();
        }
    }
}
