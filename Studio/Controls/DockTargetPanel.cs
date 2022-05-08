using Studio.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Studio.Controls
{
    [StyleTypedProperty(Property = nameof(HighlightStyle), StyleTargetType = typeof(Path))]
    public class DockTargetPanel : Control
    {
        static DockTargetPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockTargetPanel), new FrameworkPropertyMetadata(typeof(DockTargetPanel)));
        }

        #region Dependency Properties
        private static readonly DependencyPropertyKey DockHostPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(DockHost), typeof(DockContainer), typeof(DockTargetPanel), new PropertyMetadata((DockContainer)null));

        private static readonly DependencyPropertyKey DockAreaPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(DockArea), typeof(Rect), typeof(DockTargetPanel), new PropertyMetadata(Rect.Empty));

        private static readonly DependencyPropertyKey TargetHostPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(TargetHost), typeof(FrameworkElement), typeof(DockTargetPanel), new PropertyMetadata((TabWellBase)null));

        private static readonly DependencyPropertyKey TargetAreaPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(TargetArea), typeof(Rect), typeof(DockTargetPanel), new PropertyMetadata(Rect.Empty));

        private static readonly DependencyPropertyKey CanDockOuterPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(CanDockOuter), typeof(bool), typeof(DockTargetPanel), new PropertyMetadata(false));

        private static readonly DependencyPropertyKey CanDockTargetPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(CanDockTarget), typeof(bool), typeof(DockTargetPanel), new PropertyMetadata(false));

        private static readonly DependencyPropertyKey CanSplitLeftPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(CanSplitLeft), typeof(bool), typeof(DockTargetPanel), new PropertyMetadata(false));

        private static readonly DependencyPropertyKey CanSplitTopPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(CanSplitTop), typeof(bool), typeof(DockTargetPanel), new PropertyMetadata(false));

        private static readonly DependencyPropertyKey CanSplitRightPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(CanSplitRight), typeof(bool), typeof(DockTargetPanel), new PropertyMetadata(false));

        private static readonly DependencyPropertyKey CanSplitBottomPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(CanSplitBottom), typeof(bool), typeof(DockTargetPanel), new PropertyMetadata(false));

        private static readonly DependencyPropertyKey CanDropCenterPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(CanDropCenter), typeof(bool), typeof(DockTargetPanel), new PropertyMetadata(false));

        private static readonly DependencyPropertyKey HighlightPathPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(HighlightPath), typeof(Geometry), typeof(DockTargetPanel), new PropertyMetadata(Geometry.Empty));

        public static readonly DependencyProperty HighlightStyleProperty =
            DependencyProperty.Register(nameof(HighlightStyle), typeof(Style), typeof(DockTargetPanel), new PropertyMetadata((Style)null));

        public static readonly DependencyProperty DockHostProperty = DockHostPropertyKey.DependencyProperty;
        public static readonly DependencyProperty DockAreaProperty = DockAreaPropertyKey.DependencyProperty;
        public static readonly DependencyProperty TargetHostProperty = TargetHostPropertyKey.DependencyProperty;
        public static readonly DependencyProperty TargetAreaProperty = TargetAreaPropertyKey.DependencyProperty;
        public static readonly DependencyProperty CanDockOuterProperty = CanDockOuterPropertyKey.DependencyProperty;
        public static readonly DependencyProperty CanDockTargetProperty = CanDockTargetPropertyKey.DependencyProperty;
        public static readonly DependencyProperty CanSplitLeftProperty = CanSplitLeftPropertyKey.DependencyProperty;
        public static readonly DependencyProperty CanSplitTopProperty = CanSplitTopPropertyKey.DependencyProperty;
        public static readonly DependencyProperty CanSplitRightProperty = CanSplitRightPropertyKey.DependencyProperty;
        public static readonly DependencyProperty CanSplitBottomProperty = CanSplitBottomPropertyKey.DependencyProperty;
        public static readonly DependencyProperty CanDropCenterProperty = CanDropCenterPropertyKey.DependencyProperty;
        public static readonly DependencyProperty HighlightPathProperty = HighlightPathPropertyKey.DependencyProperty;

        public DockContainer DockHost
        {
            get => (DockContainer)GetValue(DockHostProperty);
            private set => SetValue(DockHostPropertyKey, value);
        }

        public Rect DockArea
        {
            get => (Rect)GetValue(DockAreaProperty);
            private set => SetValue(DockAreaPropertyKey, value);
        }

        public FrameworkElement TargetHost
        {
            get => (FrameworkElement)GetValue(TargetHostProperty);
            private set => SetValue(TargetHostPropertyKey, value);
        }

        public Rect TargetArea
        {
            get => (Rect)GetValue(TargetAreaProperty);
            private set => SetValue(TargetAreaPropertyKey, value);
        }

        public bool CanDockOuter
        {
            get => (bool)GetValue(CanDockOuterProperty);
            private set => SetValue(CanDockOuterPropertyKey, value);
        }

        public bool CanDockTarget
        {
            get => (bool)GetValue(CanDockTargetProperty);
            private set => SetValue(CanDockTargetPropertyKey, value);
        }

        public bool CanSplitLeft
        {
            get => (bool)GetValue(CanSplitLeftProperty);
            private set => SetValue(CanSplitLeftPropertyKey, value);
        }

        public bool CanSplitTop
        {
            get => (bool)GetValue(CanSplitTopProperty);
            private set => SetValue(CanSplitTopPropertyKey, value);
        }

        public bool CanSplitRight
        {
            get => (bool)GetValue(CanSplitRightProperty);
            private set => SetValue(CanSplitRightPropertyKey, value);
        }

        public bool CanSplitBottom
        {
            get => (bool)GetValue(CanSplitBottomProperty);
            private set => SetValue(CanSplitBottomPropertyKey, value);
        }

        public bool CanDropCenter
        {
            get => (bool)GetValue(CanDropCenterProperty);
            private set => SetValue(CanDropCenterPropertyKey, value);
        }

        public Geometry HighlightPath
        {
            get => (Geometry)GetValue(HighlightPathProperty);
            private set => SetValue(HighlightPathPropertyKey, value);
        }

        public Style HighlightStyle
        {
            get => (Style)GetValue(HighlightStyleProperty);
            set => SetValue(HighlightStyleProperty, value);
        }
        #endregion

        private bool CanSplitVertical
        {
            set => CanSplitTop = CanSplitBottom = value;
        }

        private bool CanSplitHorizontal
        {
            set => CanSplitLeft = CanSplitRight = value;
        }

        internal void AlignToTarget(TargetArgs args)
        {
            Rect GetElementBounds(UIElement element) => element == null ? Rect.Empty : new Rect(element.TranslatePoint(new Point(), this), element.RenderSize);

            if (args.DockContainer != DockHost)
            {
                DockHost = args.DockContainer;
                DockArea = GetElementBounds(DockHost?.ContentHost);
            }

            var host = args.TabWell as FrameworkElement ?? args.DocumentContainer;
            if (host != TargetHost)
            {
                TargetHost = host;
                TargetArea = GetElementBounds(TargetHost);
            }

            var isAllTools = args.SourceItems.All(i => i.ItemType == TabItemType.Tool);
            CanDockOuter = isAllTools;

            if (TargetHost is DocumentWell)
            {
                CanDropCenter = true;
                CanDockTarget = isAllTools;

                var panel = TargetHost.FindVisualAncestor<DocumentContainer>();
                CanSplitHorizontal = panel?.Orientation == Orientation.Horizontal || panel?.Items.Count <= 1;
                CanSplitVertical = panel?.Orientation == Orientation.Vertical || panel?.Items.Count <= 1;
            }
            else if (TargetHost is DocumentContainer)
            {
                CanDropCenter = true;
                CanDockTarget = isAllTools;
                CanSplitHorizontal = CanSplitVertical = false;
            }
            else if (TargetHost is ToolWell)
            {
                CanDockTarget = false;
                CanSplitHorizontal = CanSplitVertical = CanDropCenter = isAllTools;
            }
            else
                CanDockTarget = CanSplitHorizontal = CanSplitVertical = CanDropCenter = false;

            DockTargetButton.UpdateCursor();
            UpdateHighlightPath(args.SourceItems, args.TabItem);
        }

        private void UpdateHighlightPath(IEnumerable<TabWellItem> sourceTabs, TabWellItem item)
        {
            var dock = DockTargetButton.CurrentTargetDock;
            if (!dock.HasValue && item == null)
            {
                HighlightPath = Geometry.Empty;
                return;
            }

            var col = new PointCollection();
            if (dock.HasValue && dock.Value != DockTarget.Center)
            {
                if (!HighlightSplit(col, sourceTabs, item))
                    return;
            }
            else if (CanDropCenter) //mouse over center or specific tab
                HighlightCenter(col, sourceTabs, item);
            else
            {
                HighlightPath = Geometry.Empty;
                return;
            }

            var segs = new List<PathSegment> { new PolyLineSegment(col, true) };
            var geom = new PathGeometry();
            geom.Figures.Add(new PathFigure(col[0], segs, true));

            HighlightPath = geom;
        }

        private void HighlightCenter(PointCollection col, IEnumerable<TabWellItem> sourceTabs, TabWellItem item)
        {
            var relativeOrigin = (UIElement)DockHost?.ContentHost ?? Window.GetWindow(TargetHost);
            var hostOffset = TargetHost.TranslatePoint(new Point(), relativeOrigin);

            if (TargetHost is DocumentContainer)
            {
                col.Add(hostOffset);
                col.Add(hostOffset.X + TargetArea.Width, hostOffset.Y);
                col.Add(hostOffset.X + TargetArea.Width, hostOffset.Y + TargetArea.Height);
                col.Add(hostOffset.X, hostOffset.Y + TargetArea.Height);
            }
            else
            {
                var well = TargetHost as TabWellBase;
                var first = well.FirstContainer;
                if (item == null)
                    item = first;

                var firstOffset = first?.TranslatePoint(new Point(), relativeOrigin) ?? hostOffset;
                var itemOffset = item?.TranslatePoint(new Point(), relativeOrigin) ?? hostOffset;
                var itemHeight = sourceTabs.Max(t => t.ActualHeight);
                var itemWidth = sourceTabs.Sum(t => t.ActualWidth);

                if (TargetHost is DocumentWell)
                {
                    col.Add(hostOffset.X, firstOffset.Y + itemHeight); //well top-left
                    col.Add(itemOffset.X, itemOffset.Y + itemHeight); //item bottom-left
                    col.Add(itemOffset.X, itemOffset.Y); //item top-left
                    col.Add(itemOffset.X + itemWidth, itemOffset.Y); //item top-right
                    col.Add(itemOffset.X + itemWidth, itemOffset.Y + itemHeight); //item bottom-right
                    col.Add(hostOffset.X + TargetHost.ActualWidth, itemOffset.Y + itemHeight); //well top-right
                    col.Add(hostOffset.X + TargetHost.ActualWidth, hostOffset.Y + TargetHost.ActualHeight); //well bottom-right
                    col.Add(hostOffset.X, hostOffset.Y + TargetHost.ActualHeight); //well bottom-left
                }
                else
                {
                    col.Add(hostOffset.X, hostOffset.Y); //well top-left
                    col.Add(hostOffset.X + TargetHost.ActualWidth, hostOffset.Y); //well top-right
                    col.Add(hostOffset.X + TargetHost.ActualWidth, itemOffset.Y); //well bottom-right
                    if (well.Items.Count > 1) //tab panel visible
                    {
                        col.Add(itemOffset.X + itemWidth, itemOffset.Y); //item top-right
                        col.Add(itemOffset.X + itemWidth, itemOffset.Y + itemHeight); //item bottom-right
                        col.Add(itemOffset.X, itemOffset.Y + itemHeight); //item bottom-left
                        col.Add(itemOffset.X, itemOffset.Y); //item top-left
                    }
                    col.Add(hostOffset.X, itemOffset.Y); //well bottom-left
                }
            }
        }

        private bool HighlightSplit(PointCollection col, IEnumerable<TabWellItem> sourceTabs, TabWellItem item)
        {
            if (!(DockTargetButton.CurrentTargetHost is FrameworkElement panel))
            {
                HighlightPath = Geometry.Empty;
                return false;
            }

            var dock = DockTargetButton.CurrentTargetDock.Value;
            var wnd = Window.GetWindow(sourceTabs.First());

            double width, height, xOffset, yOffset;
            if (dock.GetDockOrientation() == Orientation.Horizontal)
            {
                width = Math.Min(wnd.ActualWidth, panel.ActualWidth / 2);
                height = panel.ActualHeight;

                xOffset = dock.GetDockSide() == Dock.Right ? panel.ActualWidth - width : 0;
                yOffset = 0;
            }
            else
            {
                width = panel.ActualWidth;
                height = Math.Min(wnd.ActualHeight, panel.ActualHeight / 2);

                xOffset = 0;
                yOffset = dock.GetDockSide() == Dock.Bottom ? panel.ActualHeight - height : 0;
            }

            var relativeOrigin = (UIElement)DockHost?.ContentHost ?? Window.GetWindow(TargetHost);
            var panelOffset = panel.TranslatePoint(new Point(), relativeOrigin);

            col.Add(panelOffset.X + xOffset, panelOffset.Y + yOffset);
            col.Add(panelOffset.X + xOffset + width, panelOffset.Y + yOffset);
            col.Add(panelOffset.X + xOffset + width, panelOffset.Y + yOffset + height);
            col.Add(panelOffset.X + xOffset, panelOffset.Y + yOffset + height);

            return true;
        }

        internal void ClearTarget()
        {
            DockHost = null;
            TargetHost = null;
            DockArea = TargetArea = Rect.Empty;
        }
    }
}
