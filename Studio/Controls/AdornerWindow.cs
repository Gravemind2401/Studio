using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Studio.Controls
{
    internal class AdornerWindow : Window
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static AdornerWindow()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(AdornerWindow), new FrameworkPropertyMetadata(typeof(AdornerWindow)));
            AllowsTransparencyProperty.OverrideMetadata(typeof(AdornerWindow), new FrameworkPropertyMetadata(true));
            ShowInTaskbarProperty.OverrideMetadata(typeof(AdornerWindow), new FrameworkPropertyMetadata(false));
            WindowStyleProperty.OverrideMetadata(typeof(AdornerWindow), new FrameworkPropertyMetadata(WindowStyle.None));
            ResizeModeProperty.OverrideMetadata(typeof(AdornerWindow), new FrameworkPropertyMetadata(ResizeMode.NoResize));
            TopmostProperty.OverrideMetadata(typeof(AdornerWindow), new FrameworkPropertyMetadata(true));
        }

        private DockTargetPanel Panel => Content as DockTargetPanel;

        private AdornerWindow(Window owner)
        {
            Background = null;

            Owner = owner;
            Owner.LocationChanged += Owner_LocationChanged;
            Owner.SizeChanged += Owner_SizeChanged;
            Owner.StateChanged += Owner_StateChanged;
            Owner.Closed += Target_Closed;
            AlignToOwner();
        }

        public static void Follow(Window target)
        {
            var wnd = new AdornerWindow(target);
            wnd.Content = new DockTargetPanel { Visibility = Visibility.Collapsed };
            wnd.Show();
        }

        private void AlignToOwner()
        {
            Top = Owner.Top;
            Left = Owner.Left;
            Width = Owner.ActualWidth;
            Height = Owner.ActualHeight;
        }

        public void SetTarget(IEnumerable<TabWellItem> sourceTabs, DockContainer container, TabWellBase well, TabWellItem item)
        {
            Panel.Visibility = Visibility.Visible;
            Panel.AlignToTarget(sourceTabs, container, well, item);
        }

        public void ClearTarget()
        {
            Panel.ClearTarget();
            Panel.Visibility = Visibility.Collapsed;
        }

        private void Owner_LocationChanged(object sender, EventArgs e)
        {
            AlignToOwner();
        }

        private void Owner_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AlignToOwner();
        }

        private void Owner_StateChanged(object sender, EventArgs e)
        {
            WindowState = Owner.WindowState;
        }

        private void Target_Closed(object sender, EventArgs e)
        {
            Owner.LocationChanged += Owner_LocationChanged;
            Owner.SizeChanged -= Owner_SizeChanged;
            Owner.StateChanged -= Owner_StateChanged;
            Close();
        }
    }
}
