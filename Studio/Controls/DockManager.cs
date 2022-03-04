using Studio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace Studio.Controls
{
    public static class DockManager
    {
        #region DockGroup
        public static readonly DependencyProperty DockGroupProperty =
            DependencyProperty.RegisterAttached("DockGroup", typeof(string), typeof(DockManager), new PropertyMetadata(string.Empty), ValidateDockGroup);

        public static string GetDockGroup(DependencyObject obj)
        {
            return (string)obj.GetValue(DockGroupProperty);
        }

        public static void SetDockGroup(DependencyObject obj, string value)
        {
            obj.SetValue(DockGroupProperty, value);
        }

        public static bool ValidateDockGroup(object value)
        {
            return value as string != null;
        }
        #endregion

        #region IsActive
        private static readonly Dictionary<string, DependencyObject> activeObjects = new Dictionary<string, DependencyObject>();

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.RegisterAttached("IsActive", typeof(bool), typeof(DockManager), new FrameworkPropertyMetadata(false, IsActiveChanged) { BindsTwoWayByDefault = true });

        public static bool GetIsActive(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsActiveProperty);
        }

        public static void SetIsActive(DependencyObject obj, bool value)
        {
            obj.SetValue(IsActiveProperty, value);
        }

        public static void IsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var group = GetDockGroup(d);
            if (!(bool)e.NewValue)
            {
                activeObjects.Remove(group);
                return;
            }

            if (activeObjects.ContainsKey(group))
                SetIsActive(activeObjects[group], false);

            if (!activeObjects.ContainsKey(group))
                activeObjects.Add(group, d);
        }
        #endregion

        #region IsPinned
        public static readonly DependencyProperty IsPinnedProperty =
            DependencyProperty.RegisterAttached("IsPinned", typeof(bool), typeof(DockManager), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static bool GetIsPinned(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsPinnedProperty);
        }

        public static void SetIsPinned(DependencyObject obj, bool value)
        {
            obj.SetValue(IsPinnedProperty, value);
        }

        #endregion

        #region Tracking

        private static readonly Dictionary<Window, List<UIElement>> trackedElements = new Dictionary<Window, List<UIElement>>();
        private static readonly Dictionary<UIElement, Window> windowLookup = new Dictionary<UIElement, Window>();
        private static readonly List<WindowInfo> windowData = new List<WindowInfo>();
        private static WindowInfo currentTarget;

        internal static void Register(UIElement element)
        {
            var wnd = Window.GetWindow(element);
            if (wnd == null || DesignerProperties.GetIsInDesignMode(element))
                return;

            if (!trackedElements.ContainsKey(wnd))
            {
                trackedElements.Add(wnd, new List<UIElement>());
                TrackWindow(wnd);
            }
            windowLookup.Add(element, wnd);
            trackedElements[wnd].Add(element);
        }

        internal static void Unregister(UIElement element)
        {
            var wnd = windowLookup[element];
            windowLookup.Remove(element);
            trackedElements[wnd].Remove(element);
            if (trackedElements[wnd].Count == 0)
                trackedElements.Remove(wnd);
        }

        private static void TrackWindow(Window wnd)
        {
            AdornerWindow.ForTarget(wnd);

            if (wnd == Application.Current.MainWindow)
                return;

            var hwnd = PresentationSource.FromVisual(wnd) as HwndSource;
            hwnd.RemoveHook(WndProc);
            hwnd.AddHook(WndProc);
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var wnd = HwndSource.FromHwnd(hwnd).RootVisual as Window;
            var pos = NativeMethods.GetMousePosition();

            try
            {
                if (msg == WindowsMessage.WM_ENTERSIZEMOVE)
                    OnDragStart(wnd);
                else if (msg == WindowsMessage.WM_MOVING)
                    OnDragMove(wnd, pos);
                else if (msg == WindowsMessage.WM_EXITSIZEMOVE)
                {
                    if (OnDragStop(wnd, pos))
                        handled = true;
                }

                return IntPtr.Zero;
            }
            catch
            {
                System.Diagnostics.Debugger.Break();
                return IntPtr.Zero;
            }
        }

        private static void OnDragStart(Window wnd)
        {
            windowData.Clear();
            foreach (var w in NativeMethods.SortWindowsTopToBottom(trackedElements.Keys.Where(w => w.WindowState != WindowState.Minimized)))
            {
                if (w == wnd)
                    continue;

                var info = new WindowInfo(w);
                windowData.Add(info);
                info.Adorner.Show();
            }

            wnd.BringToFront();
        }

        private static void OnDragMove(Window wnd, Point pos)
        {
            if (currentTarget == null)
                currentTarget = windowData.FirstOrDefault(d => d.WindowBounds.Contains(pos));

            if (currentTarget == null)
                return;

            if (!currentTarget.WindowBounds.Contains(pos))
            {
                currentTarget.Adorner.ClearTargetParams();
                currentTarget = null;
                OnDragMove(wnd, pos);
                return;
            }

            currentTarget.Adorner.SetTargetParams(GetTargetArgs(wnd, pos));
        }

        private static bool OnDragStop(Window wnd, Point pos)
        {
            foreach (var w in windowData)
                w.Adorner.Hide();

            if (currentTarget == null)
                return false;

            var well = currentTarget.WellBounds.FirstOrDefault(t => t.Item1.Contains(pos))?.Item2;
            var tab = currentTarget.TabBounds.FirstOrDefault(t => t.Item1.Contains(pos))?.Item2;
            var targetElement = tab != null ? well : DockTargetButton.CurrentTargetHost;

            if (targetElement?.DockCommand != null)
            {
                var sourceItems = trackedElements[wnd].OfType<TabWellBase>();
                var args = new DockEventArgs(sourceItems, targetElement as FrameworkElement, DockTargetButton.CurrentTargetDock ?? DockTarget.Center, tab?.GetContainerContext() ?? tab);
                targetElement.DockCommand.TryExecute(args);
            }

            currentTarget.Adorner.ClearTargetParams();
            currentTarget = null;

            return true;
        }

        private static TargetArgs GetTargetArgs(Window wnd, Point pos)
        {
            var container = currentTarget.DockBounds?.Item1.Contains(pos) == true ? currentTarget.DockBounds.Item2 : null;
            var docPanel = currentTarget.DocumentBounds?.Item1.Contains(pos) == true ? currentTarget.DocumentBounds.Item2 : null;
            var well = currentTarget.WellBounds.FirstOrDefault(t => t.Item1.Contains(pos))?.Item2;
            var tab = currentTarget.TabBounds.FirstOrDefault(t => t.Item1.Contains(pos))?.Item2;

            return new TargetArgs
            {
                DockContainer = container,
                DocumentContainer = docPanel,
                TabWell = well,
                TabItem = tab,
                SourceItems = trackedElements[wnd].OfType<TabWellItem>()
            };
        }

        #endregion

        //see https://stackoverflow.com/questions/4998076/getting-the-location-of-a-control-relative-to-the-entire-screen if scaling becomes an issue
        private class WindowInfo
        {
            public Window Window { get; }
            public AdornerWindow Adorner { get; }
            public Rect WindowBounds { get; }
            public Tuple<Rect, DockContainer> DockBounds { get; }
            public Tuple<Rect, DocumentContainer> DocumentBounds { get; }
            public List<Tuple<Rect, TabWellBase>> WellBounds { get; }
            public List<Tuple<Rect, TabWellItem>> TabBounds { get; }

            public WindowInfo(Window wnd)
            {
                Window = wnd;
                Adorner = AdornerWindow.FromTarget(wnd);
                WindowBounds = new Rect(wnd.PointToScreenScaled(new Point()), wnd.RenderSize);

                var container = trackedElements[wnd].OfType<DockContainer>().FirstOrDefault();
                if (container?.DockCommand != null)
                    DockBounds = Tuple.Create(new Rect(container.PointToScreenScaled(new Point()), container.RenderSize), container);

                var docPanel = trackedElements[wnd].OfType<DocumentContainer>().FirstOrDefault();
                if (docPanel?.DockCommand != null)
                    DocumentBounds = Tuple.Create(new Rect(docPanel.PointToScreenScaled(new Point()), docPanel.RenderSize), docPanel);

                WellBounds = new List<Tuple<Rect, TabWellBase>>();
                foreach (var w in trackedElements[wnd].OfType<TabWellBase>().Where(w => w.IsVisible && w.DockCommand != null))
                    WellBounds.Add(Tuple.Create(new Rect(w.PointToScreenScaled(new Point()), w.RenderSize), w));

                TabBounds = new List<Tuple<Rect, TabWellItem>>();
                foreach (var g in trackedElements[wnd].OfType<TabWellItem>().Where(t => t.IsVisible && t.ActualHeight > 0 && t.ActualWidth > 0).GroupBy(t => t.FindVisualAncestor<TabWellBase>()))
                {
                    if (!WellBounds.Any(b => b.Item2 == g.Key))
                        continue;

                    foreach (var t in g)
                        TabBounds.Add(Tuple.Create(new Rect(t.PointToScreenScaled(new Point()), t.RenderSize), t));
                }
            }
        }

        private static class WindowsMessage
        {
            /// <summary>Sent after a window has been moved.</summary>
            public const int WM_MOVE = 0x0003;

            /// <summary>
            /// Sent to a window when the size or position of the window is about to change.
            /// An application can use this message to override the window's default maximized size and position;
            /// or its default minimum or maximum tracking size.
            /// </summary>
            public const int WM_GETMINMAXINFO = 0x0024;

            /// <summary>
            /// Sent to a window whose size; position; or place in the Z order is about to change as a result
            /// of a call to the SetWindowPos function or another window-management function.
            /// </summary>
            public const int WM_WINDOWPOSCHANGING = 0x0046;

            /// <summary>
            /// Sent to a window whose size; position; or place in the Z order has changed as a result of a
            /// call to the SetWindowPos function or another window-management function.
            /// </summary>
            public const int WM_WINDOWPOSCHANGED = 0x0047;

            /// <summary>
            /// Sent to a window that the user is moving. By processing this message; an application can monitor
            /// the position of the drag rectangle and; if needed; change its position.
            /// </summary>
            public const int WM_MOVING = 0x0216;

            /// <summary>
            /// Sent once to a window after it enters the moving or sizing modal loop. The window enters the
            /// moving or sizing modal loop when the user clicks the window's title bar or sizing border; or
            /// when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam
            /// parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete
            /// when DefWindowProc returns.
            /// <para />
            /// The system sends the WM_ENTERSIZEMOVE message regardless of whether the dragging of full windows
            /// is enabled.
            /// </summary>
            public const int WM_ENTERSIZEMOVE = 0x0231;

            /// <summary>
            /// Sent once to a window once it has exited moving or sizing modal loop. The window enters the
            /// moving or sizing modal loop when the user clicks the window's title bar or sizing border, or
            /// when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the
            /// wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is
            /// complete when DefWindowProc returns.
            /// </summary>
            public const int WM_EXITSIZEMOVE = 0x0232;
        }
    }

    internal class TargetArgs
    {
        public DockContainer DockContainer { get; set; }
        public DocumentContainer DocumentContainer { get; set; }
        public TabWellBase TabWell { get; set; }
        public TabWellItem TabItem { get; set; }
        public IEnumerable<TabWellItem> SourceItems { get; set; }
    }
}
