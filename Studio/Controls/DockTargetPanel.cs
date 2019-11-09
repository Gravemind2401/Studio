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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
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
            DependencyProperty.RegisterReadOnly(nameof(TargetHost), typeof(TabWellBase), typeof(DockTargetPanel), new PropertyMetadata((TabWellBase)null));

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
            get { return (DockContainer)GetValue(DockHostProperty); }
            private set { SetValue(DockHostPropertyKey, value); }
        }

        public Rect DockArea
        {
            get { return (Rect)GetValue(DockAreaProperty); }
            private set { SetValue(DockAreaPropertyKey, value); }
        }

        public TabWellBase TargetHost
        {
            get { return (TabWellBase)GetValue(TargetHostProperty); }
            private set { SetValue(TargetHostPropertyKey, value); }
        }

        public Rect TargetArea
        {
            get { return (Rect)GetValue(TargetAreaProperty); }
            private set { SetValue(TargetAreaPropertyKey, value); }
        }

        public bool CanDockOuter
        {
            get { return (bool)GetValue(CanDockOuterProperty); }
            private set { SetValue(CanDockOuterPropertyKey, value); }
        }

        public bool CanDockTarget
        {
            get { return (bool)GetValue(CanDockTargetProperty); }
            private set { SetValue(CanDockTargetPropertyKey, value); }
        }

        public bool CanSplitLeft
        {
            get { return (bool)GetValue(CanSplitLeftProperty); }
            private set { SetValue(CanSplitLeftPropertyKey, value); }
        }

        public bool CanSplitTop
        {
            get { return (bool)GetValue(CanSplitTopProperty); }
            private set { SetValue(CanSplitTopPropertyKey, value); }
        }

        public bool CanSplitRight
        {
            get { return (bool)GetValue(CanSplitRightProperty); }
            private set { SetValue(CanSplitRightPropertyKey, value); }
        }

        public bool CanSplitBottom
        {
            get { return (bool)GetValue(CanSplitBottomProperty); }
            private set { SetValue(CanSplitBottomPropertyKey, value); }
        }

        public bool CanDropCenter
        {
            get { return (bool)GetValue(CanDropCenterProperty); }
            private set { SetValue(CanDropCenterPropertyKey, value); }
        }

        public Geometry HighlightPath
        {
            get { return (Geometry)GetValue(HighlightPathProperty); }
            private set { SetValue(HighlightPathPropertyKey, value); }
        }

        public Style HighlightStyle
        {
            get { return (Style)GetValue(HighlightStyleProperty); }
            set { SetValue(HighlightStyleProperty, value); }
        }
        #endregion

        private bool CanSplitVertical
        {
            set { CanSplitTop = CanSplitBottom = value; }
        }

        private bool CanSplitHorizontal
        {
            set { CanSplitLeft = CanSplitRight = value; }
        }

        internal void AlignToTarget(IEnumerable<TabWellItem> sourceTabs, DockContainer container, TabWellBase well, TabWellItem item)
        {
            if (container != DockHost)
            {
                DockHost = container;
                if (DockHost == null)
                    DockArea = Rect.Empty;
                else
                {
                    DockArea = new Rect(
                        DockHost.ContentHost.TranslatePoint(new Point(), this),
                        DockHost.ContentHost.RenderSize
                    );
                }
            }

            if (well != TargetHost)
            {
                TargetHost = well;
                if (TargetHost == null)
                    TargetArea = Rect.Empty;
                else
                {
                    TargetArea = new Rect(
                        TargetHost.TranslatePoint(new Point(), this),
                        TargetHost.RenderSize
                    );
                }
            }

            var isAllTools = sourceTabs.All(i => i.ItemType == TabItemType.Tool);
            CanDockOuter = isAllTools;

            if (well is DocumentWell)
            {
                CanDropCenter = true;
                CanDockTarget = isAllTools;

                var panel = well.Parent as DocumentContainer;
                CanSplitHorizontal = panel?.Orientation == Orientation.Horizontal || panel?.Items.Count <= 1;
                CanSplitVertical = panel?.Orientation == Orientation.Vertical || panel?.Items.Count <= 1;
            }
            else
            {
                CanDockTarget = false;
                CanSplitHorizontal = CanSplitVertical = CanDropCenter = isAllTools;
            }

            DockTargetButton.UpdateCursor();
            UpdateHighlightPath(sourceTabs, item);
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
                HighlightPath = Geometry.Empty;
                return;
            }
            else if (CanDropCenter) //mouse over center or specific tab
            {
                var first = TargetHost.FirstContainer;
                if (item == null) item = first;

                var relativeOrigin = (UIElement)DockHost?.ContentHost ?? Window.GetWindow(TargetHost);

                var wellOffset = TargetHost.TranslatePoint(new Point(), relativeOrigin);
                var firstOffset = first?.TranslatePoint(new Point(), relativeOrigin) ?? wellOffset;
                var itemOffset = item?.TranslatePoint(new Point(), relativeOrigin) ?? wellOffset;
                var itemHeight = sourceTabs.Max(t => t.ActualHeight);
                var itemWidth = sourceTabs.Sum(t => t.ActualWidth);

                if (TargetHost is DocumentWell)
                {
                    col.Add(new Point(wellOffset.X, firstOffset.Y + itemHeight)); //well top-left
                    col.Add(new Point(itemOffset.X, itemOffset.Y + itemHeight)); //item bottom-left
                    col.Add(new Point(itemOffset.X, itemOffset.Y)); //item top-left
                    col.Add(new Point(itemOffset.X + itemWidth, itemOffset.Y)); //item top-right
                    col.Add(new Point(itemOffset.X + itemWidth, itemOffset.Y + itemHeight)); //item bottom-right
                    col.Add(new Point(wellOffset.X + TargetHost.ActualWidth, itemOffset.Y + itemHeight)); //well top-right
                    col.Add(new Point(wellOffset.X + TargetHost.ActualWidth, wellOffset.Y + TargetHost.ActualHeight)); //well bottom-right
                    col.Add(new Point(wellOffset.X, wellOffset.Y + TargetHost.ActualHeight)); //well bottom-left
                }
                else
                {
                    col.Add(new Point(wellOffset.X, wellOffset.Y)); //well top-left
                    col.Add(new Point(wellOffset.X + TargetHost.ActualWidth, wellOffset.Y)); //well top-right
                    col.Add(new Point(wellOffset.X + TargetHost.ActualWidth, itemOffset.Y)); //well bottom-right
                    if (TargetHost.Items.Count > 1) //tab panel visible
                    {
                        col.Add(new Point(itemOffset.X + itemWidth, itemOffset.Y)); //item top-right
                        col.Add(new Point(itemOffset.X + itemWidth, itemOffset.Y + itemHeight)); //item bottom-right
                        col.Add(new Point(itemOffset.X, itemOffset.Y + itemHeight)); //item bottom-left
                        col.Add(new Point(itemOffset.X, itemOffset.Y)); //item top-left
                    }
                    col.Add(new Point(wellOffset.X, itemOffset.Y)); //well bottom-left
                }
            }
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

        internal void ClearTarget()
        {
            DockHost = null;
            TargetHost = null;

            DockArea = TargetArea = Rect.Empty;
        }
    }
}
