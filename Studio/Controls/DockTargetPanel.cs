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

        private static readonly DependencyPropertyKey CanSplitTargetPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(CanSplitTarget), typeof(bool), typeof(DockTargetPanel), new PropertyMetadata(false));

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
        public static readonly DependencyProperty CanSplitTargetProperty = CanSplitTargetPropertyKey.DependencyProperty;
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

        public bool CanSplitTarget
        {
            get { return (bool)GetValue(CanSplitTargetProperty); }
            private set { SetValue(CanSplitTargetPropertyKey, value); }
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
                        DockHost.TranslatePoint(new Point(), this),
                        DockHost.RenderSize
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

            DockTargetButton.UpdateCursor();

            var isAllTools = sourceTabs.All(i => i.ItemType == TabItemType.Tool);
            CanDockOuter = isAllTools;
            CanDockTarget = well is DocumentWell && isAllTools;
            CanSplitTarget = well is DocumentWell || isAllTools;

            UpdateHighlightPath(sourceTabs, container, well, item);
        }

        private void UpdateHighlightPath(IEnumerable<TabWellItem> sourceTabs, DockContainer container, TabWellBase well, TabWellItem item)
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
            else
            {
                var first = well.FirstContainer;
                if (item == null) item = first;

                var wellOffset = well.TranslatePoint(new Point(), container);
                var firstOffset = first?.TranslatePoint(new Point(), container) ?? wellOffset;
                var itemOffset = item?.TranslatePoint(new Point(), container) ?? wellOffset;
                var itemHeight = sourceTabs.Max(t => t.ActualHeight);
                var itemWidth = sourceTabs.Sum(t => t.ActualWidth);

                col.Add(new Point(wellOffset.X, firstOffset.Y + itemHeight));
                col.Add(new Point(itemOffset.X, itemOffset.Y + itemHeight));
                col.Add(new Point(itemOffset.X, itemOffset.Y));
                col.Add(new Point(itemOffset.X + itemWidth, itemOffset.Y));
                col.Add(new Point(itemOffset.X + itemWidth, itemOffset.Y + itemHeight));
                col.Add(new Point(wellOffset.X + well.ActualWidth, itemOffset.Y + itemHeight));
                col.Add(new Point(wellOffset.X + well.ActualWidth, wellOffset.Y + well.ActualHeight));
                col.Add(new Point(wellOffset.X, wellOffset.Y + well.ActualHeight));
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
