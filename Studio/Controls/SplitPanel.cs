using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Studio.Controls
{
    public class SplitPanel : Panel
    {
        #region Attached Dependency Properties
        public static readonly DependencyProperty DesiredSizeProperty =
            DependencyProperty.RegisterAttached("DesiredSize", typeof(GridLength), typeof(SplitPanel), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public static GridLength GetDesiredSize(DependencyObject obj)
        {
            return (GridLength)obj.GetValue(DesiredSizeProperty);
        }

        public static void SetDesiredSize(DependencyObject obj, GridLength value)
        {
            obj.SetValue(DesiredSizeProperty, value);
        }

        public static readonly DependencyProperty MinSizeProperty =
            DependencyProperty.RegisterAttached("MinSize", typeof(double), typeof(SplitPanel), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public static double GetMinSize(DependencyObject obj)
        {
            return (double)obj.GetValue(MinSizeProperty);
        }

        public static void SetMinSize(DependencyObject obj, double value)
        {
            obj.SetValue(MinSizeProperty, value);
        }

        public static readonly DependencyProperty MaxSizeProperty =
            DependencyProperty.RegisterAttached("MaxSize", typeof(double), typeof(SplitPanel), new FrameworkPropertyMetadata(double.PositiveInfinity, FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public static double GetMaxSize(DependencyObject obj)
        {
            return (double)obj.GetValue(MaxSizeProperty);
        }

        public static void SetMaxSize(DependencyObject obj, double value)
        {
            obj.SetValue(MaxSizeProperty, value);
        }

        #endregion

        #region Dependency Properties
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(SplitPanel), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty SplitterThicknessProperty =
            DependencyProperty.Register(nameof(SplitterThickness), typeof(double), typeof(SplitPanel), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public double SplitterThickness
        {
            get { return (double)GetValue(SplitterThicknessProperty); }
            set { SetValue(SplitterThicknessProperty, value); }
        }
        #endregion

        private readonly UIElementCollection splitters;

        public SplitPanel()
        {
            splitters = new UIElementCollection(this, null);
        }

        private void UpdateSplitters()
        {
            var desiredCount = Children.Count - 1;
            while (splitters.Count > desiredCount)
                splitters.RemoveAt(splitters.Count - 1);

            while (splitters.Count < desiredCount)
                splitters.Add(new SplitPanelSplitter(this, splitters.Count));
        }

        private Dictionary<UIElement, Size> SizeElements(Size availableSize)
        {
            var result = new Dictionary<UIElement, Size>();

            var remaining = Orientation == Orientation.Horizontal ? availableSize.Width : availableSize.Height;
            remaining -= splitters.Count * SplitterThickness;

            var segments = InternalChildren.OfType<UIElement>()
                .Select(e => new { Element = e, Size = GetDesiredSize(e) });

            var totalFixed = segments.Where(e => e.Size.IsAbsolute).Sum(e => e.Size.Value);
            var totalAuto = segments.Where(e => e.Size.IsAuto).Sum(e => Orientation == Orientation.Horizontal ? e.Element.DesiredSize.Width : e.Element.DesiredSize.Height);
            var totalStar = segments.Where(e => e.Size.IsStar).Sum(e => e.Size.Value);

            var splitterSize = Orientation == Orientation.Horizontal
                ? new Size(SplitterThickness, availableSize.Height)
                : new Size(availableSize.Width, SplitterThickness);

            var allocatedFixed = Math.Min(totalFixed, remaining);
            var allocatedAuto = Math.Min(totalAuto, remaining - allocatedFixed);
            var allocatedStar = Math.Max(0, remaining - (allocatedFixed + allocatedAuto));

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                var child = InternalChildren[i];
                var desired = GetDesiredSize(child);

                double actual;
                if (desired.IsAbsolute)
                    actual = (desired.Value / totalFixed) * allocatedFixed;
                else if (desired.IsAuto)
                {
                    var fraction = Orientation == Orientation.Horizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
                    actual = (fraction / totalFixed) * allocatedAuto;
                }
                else //desired.IsStar
                    actual = (desired.Value / totalStar) * allocatedStar;

                var childSize = Orientation == Orientation.Horizontal
                    ? new Size(actual, availableSize.Height)
                    : new Size(availableSize.Width, actual);

                result.Add(child, childSize);
            }

            return result;
        }

        #region Overrides
        protected override int VisualChildrenCount => base.VisualChildrenCount + splitters.Count;

        protected override Visual GetVisualChild(int index)
        {
            return index < Children.Count ? base.GetVisualChild(index) : splitters[index - Children.Count];
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            if (!(visualAdded is SplitPanelSplitter || visualRemoved is SplitPanelSplitter))
                UpdateSplitters();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var calculated = SizeElements(availableSize);
            foreach (var pair in calculated)
                pair.Key.Measure(pair.Value);

            var splitterSize = Orientation == Orientation.Horizontal
                ? new Size(SplitterThickness, availableSize.Height)
                : new Size(availableSize.Width, SplitterThickness);

            foreach (UIElement splitter in splitters)
                splitter.Measure(splitterSize);

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var calculated = SizeElements(finalSize);

            var splitterSize = Orientation == Orientation.Horizontal
                ? new Size(SplitterThickness, finalSize.Height)
                : new Size(finalSize.Width, SplitterThickness);

            double offset = 0;
            for (int i = 0; i < InternalChildren.Count; i++)
            {
                var child = InternalChildren[i];
                var childSize = calculated[child];
                var segmentSize = Orientation == Orientation.Horizontal ? childSize.Width : childSize.Height;

                var itemPos = Orientation == Orientation.Horizontal
                    ? new Point(offset, 0)
                    : new Point(0, offset);

                child.Arrange(new Rect(itemPos, childSize));

                if (i < splitters.Count)
                {
                    var splitterPos = Orientation == Orientation.Horizontal
                        ? new Point(offset + segmentSize, 0)
                        : new Point(0, offset + segmentSize);

                    splitters[i].Arrange(new Rect(splitterPos, splitterSize));
                }

                offset += segmentSize + SplitterThickness;
            }

            return base.ArrangeOverride(finalSize);
        } 
        #endregion
    }
}
