using Studio.Utilities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Studio.Controls
{
    [StyleTypedProperty(Property = nameof(PreviewStyle), StyleTargetType = typeof(Control))]
    public sealed class SplitPanelSplitter : Thumb
    {
        #region Static
        static SplitPanelSplitter()
        {
            EventManager.RegisterClassHandler(typeof(SplitPanelSplitter), DragStartedEvent, new DragStartedEventHandler(OnDragStarted));
            EventManager.RegisterClassHandler(typeof(SplitPanelSplitter), DragDeltaEvent, new DragDeltaEventHandler(OnDragDelta));
            EventManager.RegisterClassHandler(typeof(SplitPanelSplitter), DragCompletedEvent, new DragCompletedEventHandler(OnDragCompleted));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitPanelSplitter), new FrameworkPropertyMetadata(typeof(SplitPanelSplitter)));
            FocusableProperty.OverrideMetadata(typeof(SplitPanelSplitter), new FrameworkPropertyMetadata(true));
            CursorProperty.OverrideMetadata(typeof(SplitPanelSplitter), new FrameworkPropertyMetadata(null, CoerceCursor));
        }

        private static object CoerceCursor(DependencyObject d, object value)
        {
            return ((SplitPanelSplitter)d).Owner.Orientation == Orientation.Horizontal ? Cursors.SizeWE : Cursors.SizeNS;
        }
        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty ShowsPreviewProperty =
            DependencyProperty.Register(nameof(ShowsPreview), typeof(bool), typeof(SplitPanelSplitter), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty PreviewStyleProperty =
                DependencyProperty.Register(nameof(PreviewStyle), typeof(Style), typeof(SplitPanelSplitter), new FrameworkPropertyMetadata((Style)null));

        public static readonly DependencyProperty KeyboardIncrementProperty =
                DependencyProperty.Register(nameof(KeyboardIncrement), typeof(double), typeof(SplitPanelSplitter), new FrameworkPropertyMetadata(10d), IsValidDelta);

        public static readonly DependencyProperty DragIncrementProperty =
                DependencyProperty.Register(nameof(DragIncrement), typeof(double), typeof(SplitPanelSplitter), new FrameworkPropertyMetadata(1.0), IsValidDelta);

        public bool ShowsPreview
        {
            get => (bool)GetValue(ShowsPreviewProperty);
            set => SetValue(ShowsPreviewProperty, value);
        }

        public Style PreviewStyle
        {
            get => (Style)GetValue(PreviewStyleProperty);
            set => SetValue(PreviewStyleProperty, value);
        }

        public double KeyboardIncrement
        {
            get => (double)GetValue(KeyboardIncrementProperty);
            set => SetValue(KeyboardIncrementProperty, value);
        }

        public double DragIncrement
        {
            get => (double)GetValue(DragIncrementProperty);
            set => SetValue(DragIncrementProperty, value);
        }

        private static bool IsValidDelta(object o)
        {
            double delta = (double)o;
            return delta > 0.0 && !double.IsPositiveInfinity(delta);
        }

        #endregion

        private SplitPanel Owner { get; }
        private int SplitterIndex { get; }

        internal SplitPanelSplitter(SplitPanel owner, int index)
        {
            Owner = owner;
            SplitterIndex = index;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            CoerceValue(CursorProperty);
        }

        #region PreviewAdorner

        private sealed class PreviewAdorner : Adorner
        {
            private readonly TranslateTransform translation;
            private readonly Decorator decorator;

            public double OffsetX
            {
                get => translation.X;
                set => translation.X = value;
            }

            public double OffsetY
            {
                get => translation.Y;
                set => translation.Y = value;
            }

            public PreviewAdorner(SplitPanelSplitter splitter, Style previewStyle)
                : base(splitter)
            {
                var previewControl = new Control
                {
                    Style = previewStyle,
                    IsEnabled = false
                };

                translation = new TranslateTransform();
                decorator = new Decorator
                {
                    Child = previewControl,
                    RenderTransform = translation
                };

                AddVisualChild(decorator);
            }

            protected override Visual GetVisualChild(int index)
            {
                if (index != 0)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return decorator;
            }

            protected override int VisualChildrenCount => 1;

            protected override Size ArrangeOverride(Size finalSize)
            {
                decorator.Arrange(new Rect(new Point(), finalSize));
                return finalSize;
            }
        }

        private void RemovePreviewAdorner()
        {
            if (_resizeData.Adorner != null)
            {
                var layer = VisualTreeHelper.GetParent(_resizeData.Adorner) as AdornerLayer;
                layer.Remove(_resizeData.Adorner);
            }
        }

        #endregion

        #region Splitter Setup

        private void InitializeData(bool ShowsPreview)
        {
            _resizeData = new ResizeData
            {
                Panel = Owner,
                ShowsPreview = ShowsPreview,
                SplitterLength = Math.Min(ActualWidth, ActualHeight),
                SplitterIndex = SplitterIndex
            };

            SetupDefinitionsToResize();
            SetupPreview();
        }

        private void SetupDefinitionsToResize()
        {
            var index1 = SplitterIndex;
            var index2 = SplitterIndex + 1;

            _resizeData.Definition1Index = index1;
            _resizeData.Definition1 = new SegmentDefinition(Owner, Owner.SortedChildren[index1] as FrameworkElement);
            _resizeData.OriginalDefinition1Length = _resizeData.Definition1.DesiredSize;  //save Size if user cancels
            _resizeData.OriginalDefinition1ActualLength = _resizeData.Definition1.ActualSize;

            _resizeData.Definition2Index = index2;
            _resizeData.Definition2 = new SegmentDefinition(Owner, Owner.SortedChildren[index2] as FrameworkElement);
            _resizeData.OriginalDefinition2Length = _resizeData.Definition2.DesiredSize;  //save Size if user cancels
            _resizeData.OriginalDefinition2ActualLength = _resizeData.Definition2.ActualSize;

            var isStar1 = _resizeData.Definition1.DesiredSize.IsStar;
            var isStar2 = _resizeData.Definition2.DesiredSize.IsStar;
            if (isStar1 && isStar2)
            {
                // If they are both stars, resize both
                _resizeData.SplitBehavior = SplitBehavior.ResizeBoth;
            }
            else
            {
                // One column is fixed width, resize the first one that is fixed
                _resizeData.SplitBehavior = !isStar1 ? SplitBehavior.Resize1 : SplitBehavior.Resize2;
            }
        }

        private void SetupPreview()
        {
            if (!_resizeData.ShowsPreview)
                return;

            var adornerlayer = AdornerLayer.GetAdornerLayer(_resizeData.Panel);
            if (adornerlayer == null)
                return;

            _resizeData.Adorner = new PreviewAdorner(this, PreviewStyle);
            adornerlayer.Add(_resizeData.Adorner);

            GetDeltaConstraints(out _resizeData.MinChange, out _resizeData.MaxChange);
        }

        #endregion

        #region Event Handlers

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);

            if (_resizeData != null)
                CancelResize();
        }

        private static void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            var splitter = sender as SplitPanelSplitter;
            splitter.OnDragStarted(e);
        }

        private void OnDragStarted(DragStartedEventArgs e)
        {
            InitializeData(ShowsPreview);
        }

        private static void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var splitter = sender as SplitPanelSplitter;
            splitter.OnDragDelta(e);
        }

        private void OnDragDelta(DragDeltaEventArgs e)
        {
            if (_resizeData == null)
                return;

            var horizontalChange = e.HorizontalChange;
            var verticalChange = e.VerticalChange;

            // Round change to nearest multiple of DragIncrement
            horizontalChange = Math.Round(horizontalChange / DragIncrement) * DragIncrement;
            verticalChange = Math.Round(verticalChange / DragIncrement) * DragIncrement;

            if (_resizeData.ShowsPreview)
            {
                //Set the Translation of the Adorner to the distance from the thumb
                if (_resizeData.Panel.Orientation == Orientation.Horizontal)
                    _resizeData.Adorner.OffsetX = Math.Min(Math.Max(horizontalChange, _resizeData.MinChange), _resizeData.MaxChange);
                else
                    _resizeData.Adorner.OffsetY = Math.Min(Math.Max(verticalChange, _resizeData.MinChange), _resizeData.MaxChange);
            }
            else
            {
                // Directly update the grid
                MoveSplitter(horizontalChange, verticalChange);
            }
        }

        private static void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            var splitter = sender as SplitPanelSplitter;
            splitter.OnDragCompleted(e);
        }

        private void OnDragCompleted(DragCompletedEventArgs e)
        {
            if (_resizeData == null)
                return;

            if (_resizeData.ShowsPreview)
            {
                // Update the grid
                MoveSplitter(_resizeData.Adorner.OffsetX, _resizeData.Adorner.OffsetY);
                RemovePreviewAdorner();
            }

            _resizeData = null;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    if (_resizeData != null)
                    {
                        CancelResize();
                        e.Handled = true;
                    }
                    break;

                case Key.Left:
                    e.Handled = KeyboardMoveSplitter(-KeyboardIncrement, 0);
                    break;
                case Key.Right:
                    e.Handled = KeyboardMoveSplitter(KeyboardIncrement, 0);
                    break;
                case Key.Up:
                    e.Handled = KeyboardMoveSplitter(0, -KeyboardIncrement);
                    break;
                case Key.Down:
                    e.Handled = KeyboardMoveSplitter(0, KeyboardIncrement);
                    break;
            }
        }

        private void CancelResize()
        {
            // Restore original column/row lengths
            if (_resizeData.ShowsPreview)
                RemovePreviewAdorner();
            else // Reset the columns'/rows' lengths to the saved values 
            {
                _resizeData.Definition1.DesiredSize = _resizeData.OriginalDefinition1Length;
                _resizeData.Definition2.DesiredSize = _resizeData.OriginalDefinition2Length;
            }

            _resizeData = null;
        }

        #endregion

        #region Helper Methods

        // Get the minimum and maximum Delta can be given definition constraints (MinWidth/MaxWidth)
        private void GetDeltaConstraints(out double minDelta, out double maxDelta)
        {
            var definition1Len = _resizeData.Definition1.ActualSize;
            var definition1Min = _resizeData.Definition1.MinSize;
            var definition1Max = _resizeData.Definition1.MaxSize;

            var definition2Len = _resizeData.Definition2.ActualSize;
            var definition2Min = _resizeData.Definition2.MinSize;
            var definition2Max = _resizeData.Definition2.MaxSize;

            //Set MinWidths to be greater than width of splitter
            if (_resizeData.SplitterIndex == _resizeData.Definition1Index)
                definition1Min = Math.Max(definition1Min, _resizeData.SplitterLength);
            else if (_resizeData.SplitterIndex == _resizeData.Definition2Index)
                definition2Min = Math.Max(definition2Min, _resizeData.SplitterLength);

            if (_resizeData.SplitBehavior == SplitBehavior.ResizeBoth)
            {
                // Determine the minimum and maximum the columns can be resized
                minDelta = -Math.Min(definition1Len - definition1Min, definition2Max - definition2Len);
                maxDelta = Math.Min(definition1Max - definition1Len, definition2Len - definition2Min);
            }
            else if (_resizeData.SplitBehavior == SplitBehavior.Resize1)
            {
                minDelta = definition1Min - definition1Len;
                maxDelta = definition1Max - definition1Len;
            }
            else
            {
                minDelta = definition2Len - definition2Max;
                maxDelta = definition2Len - definition2Min;
            }
        }

        //Sets the length of definition1 and definition2 
        private void SetLengths(double definition1Pixels, double definition2Pixels)
        {
            if (_resizeData.SplitBehavior == SplitBehavior.Resize1)
                _resizeData.Definition1.DesiredSize = new GridLength(definition1Pixels);
            else if (_resizeData.SplitBehavior == SplitBehavior.Resize2)
                _resizeData.Definition2.DesiredSize = new GridLength(definition2Pixels);
            else // For the case where both definition1 and 2 are stars, update all star values to match their current pixel values
            {
                int i = 0;
                foreach (FrameworkElement child in Owner.SortedChildren)
                {
                    // For each definition, if it is a star, set is value to ActualLength in stars
                    // This makes 1 star == 1 pixel in length
                    if (i == _resizeData.Definition1Index)
                        SplitPanel.SetDesiredSize(child, new GridLength(definition1Pixels, GridUnitType.Star));
                    else if (i == _resizeData.Definition2Index)
                        SplitPanel.SetDesiredSize(child, new GridLength(definition2Pixels, GridUnitType.Star));
                    else if (SplitPanel.GetDesiredSize(child).IsStar)
                    {
                        var actualSize = Owner.Orientation == Orientation.Horizontal ? child.ActualWidth : child.ActualHeight;
                        SplitPanel.SetDesiredSize(child, new GridLength(actualSize, GridUnitType.Star));
                    }

                    i++;
                }
            }
        }

        // Move the splitter by the given Delta's in the horizontal and vertical directions
        private void MoveSplitter(double horizontalChange, double verticalChange)
        {
            double delta;
            var dpi = this.GetDpi();

            if (_resizeData.Panel.Orientation == Orientation.Horizontal)
            {
                delta = horizontalChange;
                if (UseLayoutRounding)
                    delta = LayoutDoubleUtil.RoundLayoutValue(delta, dpi.DpiScaleX);
            }
            else
            {
                delta = verticalChange;
                if (UseLayoutRounding)
                    delta = LayoutDoubleUtil.RoundLayoutValue(delta, dpi.DpiScaleY);
            }

            var definition1 = _resizeData.Definition1;
            var definition2 = _resizeData.Definition2;
            if (definition1 != null && definition2 != null)
            {
                // When splitting, Check to see if the total pixels spanned by the definitions 
                // is the same as before starting resize. If not cancel the drag
                if (_resizeData.SplitBehavior == SplitBehavior.ResizeBoth &&
                    !LayoutDoubleUtil.AreClose(definition1.ActualSize + definition2.ActualSize, _resizeData.OriginalDefinition1ActualLength + _resizeData.OriginalDefinition2ActualLength))
                {
                    CancelResize();
                    return;
                }

                GetDeltaConstraints(out double min, out double max);

                // Flip when the splitter's flow direction isn't the same as the grid's
                if (FlowDirection != _resizeData.Panel.FlowDirection)
                    delta = -delta;

                delta = Math.Min(Math.Max(delta, min), max);

                var definition1LengthNew = definition1.ActualSize + delta;
                var definition2LengthNew = definition1.ActualSize + definition2.ActualSize - definition1LengthNew;

                SetLengths(definition1LengthNew, definition2LengthNew);
            }
        }

        // Move the splitter using the Keyboard (Don't show preview)
        internal bool KeyboardMoveSplitter(double horizontalChange, double verticalChange)
        {
            // If moving with the mouse, ignore keyboard motion
            if (_resizeData != null)
                return false;  // don't handle the event

            InitializeData(false);  // don't show preview

            // Check that we are actually able to resize
            if (_resizeData == null)
                return false;

            // Keyboard keys are unaffected by FlowDirection.
            if (FlowDirection == FlowDirection.RightToLeft)
                horizontalChange = -horizontalChange;

            MoveSplitter(horizontalChange, verticalChange);

            _resizeData = null;

            return true;
        }

        #endregion

        #region Data

        // Splitter has special Behavior when columns are fixed
        // If the left column is fixed, splitter will only resize that column
        // Else if the right column is fixed, splitter will only resize the right column
        private enum SplitBehavior
        {
            ResizeBoth, // Both columns/rows are star lengths
            Resize1, // resize 1 only
            Resize2, // resize 2 only
        }

        private ResizeData _resizeData;

        private class ResizeData
        {
            public bool ShowsPreview;
            public PreviewAdorner Adorner;

            public double MinChange;
            public double MaxChange;

            public SplitPanel Panel;

            public SegmentDefinition Definition1;
            public SegmentDefinition Definition2;

            public SplitBehavior SplitBehavior;

            public int SplitterIndex;

            public int Definition1Index;
            public int Definition2Index;

            public GridLength OriginalDefinition1Length;
            public GridLength OriginalDefinition2Length;
            public double OriginalDefinition1ActualLength;
            public double OriginalDefinition2ActualLength;

            public double SplitterLength;
        }

        private class SegmentDefinition
        {
            private readonly SplitPanel owner;
            private readonly FrameworkElement element;

            public SegmentDefinition(SplitPanel owner, FrameworkElement element)
            {
                this.owner = owner;
                this.element = element;
            }

            public double ActualSize => owner.Orientation == Orientation.Horizontal ? element.ActualWidth : element.ActualHeight;

            public double MinSize
            {
                get => SplitPanel.GetMinSize(element);
                set => SplitPanel.SetMinSize(element, value);
            }

            public GridLength DesiredSize
            {
                get => SplitPanel.GetDesiredSize(element);
                set => SplitPanel.SetDesiredSize(element, value);
            }

            public double MaxSize
            {
                get => SplitPanel.GetMaxSize(element);
                set => SplitPanel.SetMaxSize(element, value);
            }
        }

        #endregion
    }
}