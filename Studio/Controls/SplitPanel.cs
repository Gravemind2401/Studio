﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Studio.Controls
{
    public class SplitPanel : Panel
    {
        private const string DesiredSizePropertyName = "DesiredSize";
        private const string MinSizePropertyName = "MinSize";
        private const string MaxSizePropertyName = "MaxSize";
        private const string DisplayIndexPropertyName = "DisplayIndex";

        #region Attached Dependency Properties
        public static readonly DependencyProperty DesiredSizeProperty =
            DependencyProperty.RegisterAttached(DesiredSizePropertyName, typeof(GridLength), typeof(SplitPanel), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), FrameworkPropertyMetadataOptions.AffectsParentMeasure | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static GridLength GetDesiredSize(DependencyObject obj) => (GridLength)obj.GetValue(DesiredSizeProperty);
        public static void SetDesiredSize(DependencyObject obj, GridLength value) => obj.SetValue(DesiredSizeProperty, value);

        public static readonly DependencyProperty MinSizeProperty =
            DependencyProperty.RegisterAttached(MinSizePropertyName, typeof(double), typeof(SplitPanel), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public static double GetMinSize(DependencyObject obj) => (double)obj.GetValue(MinSizeProperty);
        public static void SetMinSize(DependencyObject obj, double value) => obj.SetValue(MinSizeProperty, value);

        public static readonly DependencyProperty MaxSizeProperty =
            DependencyProperty.RegisterAttached(MaxSizePropertyName, typeof(double), typeof(SplitPanel), new FrameworkPropertyMetadata(double.PositiveInfinity, FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public static double GetMaxSize(DependencyObject obj) => (double)obj.GetValue(MaxSizeProperty);
        public static void SetMaxSize(DependencyObject obj, double value) => obj.SetValue(MaxSizeProperty, value);

        public static readonly DependencyProperty DisplayIndexProperty =
            DependencyProperty.RegisterAttached(DisplayIndexPropertyName, typeof(int), typeof(SplitPanel), new PropertyMetadata(0, DisplayIndexChanged));

        public static int GetDisplayIndex(DependencyObject obj) => (int)obj.GetValue(DisplayIndexProperty);
        public static void SetDisplayIndex(DependencyObject obj, int value) => obj.SetValue(DisplayIndexProperty, value);

        public static void DisplayIndexChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var panel = (obj as FrameworkElement)?.Parent as SplitPanel;
            panel?.sortedChildren.Sort(panel.SortCompare);
        }
        #endregion

        #region Dependency Properties
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(SplitPanel), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public static readonly DependencyProperty SplitterThicknessProperty =
            DependencyProperty.Register(nameof(SplitterThickness), typeof(double), typeof(SplitPanel), new FrameworkPropertyMetadata(5d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public double SplitterThickness
        {
            get => (double)GetValue(SplitterThicknessProperty);
            set => SetValue(SplitterThicknessProperty, value);
        }
        #endregion

        private readonly VisualCollection splitters;

        private readonly List<UIElement> sortedChildren;
        internal IReadOnlyList<UIElement> SortedChildren => sortedChildren;

        public SplitPanel()
        {
            splitters = new VisualCollection(this);
            sortedChildren = new List<UIElement>();
        }

        private void UpdateSplitters()
        {
            var desiredCount = Children.Count - 1;
            while (splitters.Count > desiredCount)
                splitters.RemoveAt(splitters.Count - 1);

            while (splitters.Count < desiredCount)
                splitters.Add(new SplitPanelSplitter(this, splitters.Count));
        }

        private int SortCompare(UIElement a, UIElement b)
        {
            var explicitIndex = GetDisplayIndex(a).CompareTo(GetDisplayIndex(b));
            return explicitIndex != 0 ? explicitIndex : InternalChildren.IndexOf(a).CompareTo(InternalChildren.IndexOf(b));
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

            var allocatedFixed = Math.Min(totalFixed, remaining);
            var allocatedAuto = Math.Min(totalAuto, remaining - allocatedFixed);
            var allocatedStar = Math.Max(0, remaining - (allocatedFixed + allocatedAuto));

            var sorted = InternalChildren.OfType<UIElement>()
                .Select((e, i) => new { Element = e, Index = i, Sort = GetDisplayIndex(e) })
                .OrderBy(o => o.Sort)
                .ThenBy(o => o.Index)
                .Select(o => o.Element);

            foreach (var child in sorted)
            {
                var desired = GetDesiredSize(child);

                double actual;
                if (desired.IsAbsolute)
                    actual = totalFixed > 0 ? (desired.Value / totalFixed) * allocatedFixed : 0;
                else if (desired.IsAuto)
                {
                    if (totalAuto > 0)
                    {
                        var fraction = Orientation == Orientation.Horizontal ? child.DesiredSize.Width : child.DesiredSize.Height;
                        actual = (fraction / totalAuto) * allocatedAuto;
                    }
                    else
                        actual = 0;
                }
                else //desired.IsStar
                    actual = totalStar > 0 ? (desired.Value / totalStar) * allocatedStar : 0;

                actual = Math.Max(0, actual);
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
            {
                if (visualRemoved is UIElement oldElement)
                    sortedChildren.Remove(oldElement);

                if (visualAdded is UIElement newElement)
                    sortedChildren.Add(newElement);

                sortedChildren.Sort(SortCompare);
                UpdateSplitters();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var calculated = SizeElements(availableSize);
            foreach (var pair in calculated)
                pair.Key.Measure(pair.Value);

            var splitterSize = Orientation == Orientation.Horizontal
                ? new Size(SplitterThickness, availableSize.Height)
                : new Size(availableSize.Width, SplitterThickness);

            foreach (var splitter in splitters.OfType<UIElement>())
                splitter.Measure(splitterSize);

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var calculated = SizeElements(finalSize).ToList();

            var splitterSize = Orientation == Orientation.Horizontal
                ? new Size(SplitterThickness, finalSize.Height)
                : new Size(finalSize.Width, SplitterThickness);

            double offset = 0;
            for (var i = 0; i < InternalChildren.Count; i++)
            {
                var child = calculated[i].Key;
                var childSize = calculated[i].Value;
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

                    ((UIElement)splitters[i]).Arrange(new Rect(splitterPos, splitterSize));
                }

                offset += segmentSize + SplitterThickness;
            }

            return base.ArrangeOverride(finalSize);
        }
        #endregion
    }
}
