using Studio.Utilities;
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
    public enum DockTarget
    {
        Center,
        SplitLeft,
        SplitTop,
        SplitRight,
        SplitBottom,
        DockLeft,
        DockTop,
        DockRight,
        DockBottom
    }

    public class DockTargetButton : Button
    {
        private static readonly List<DockTargetButton> Instances = new List<DockTargetButton>();

        internal static DockTarget? CurrentTargetDock => Instances.FirstOrDefault(b => b.IsTargeted)?.TargetDock;

        internal static IDockReceiver CurrentTargetHost => Instances.FirstOrDefault(b => b.IsTargeted)?.TargetHost as IDockReceiver;

        internal static void UpdateCursor()
        {
            foreach (var btn in Instances)
                btn.CoerceValue(IsTargetedProperty);
        }

        public static readonly DependencyProperty TargetDockProperty =
            DependencyProperty.Register(nameof(TargetDock), typeof(DockTarget), typeof(DockTargetButton), new PropertyMetadata(DockTarget.Center));

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(DockTargetButton), new PropertyMetadata(new CornerRadius(0)));

        public static readonly DependencyProperty GlyphBorderBrushProperty =
            DependencyProperty.Register(nameof(GlyphBorderBrush), typeof(Brush), typeof(DockTargetButton), new PropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty GlyphBackgroundProperty =
            DependencyProperty.Register(nameof(GlyphBackground), typeof(Brush), typeof(DockTargetButton), new PropertyMetadata(Brushes.Transparent));

        public static readonly DependencyProperty GlyphArrowBrushProperty =
            DependencyProperty.Register(nameof(GlyphArrowBrush), typeof(Brush), typeof(DockTargetButton), new PropertyMetadata(Brushes.Black));

        private static readonly DependencyPropertyKey IsTargetedPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(IsTargeted), typeof(bool), typeof(DockTargetButton), new PropertyMetadata(false, null, CoerceIsTargeted));

        public static readonly DependencyProperty IsTargetedProperty = IsTargetedPropertyKey.DependencyProperty;

        public static readonly DependencyProperty TargetHostProperty =
            DependencyProperty.Register(nameof(TargetHost), typeof(object), typeof(DockTargetButton), new PropertyMetadata((object)null));

        public DockTarget TargetDock
        {
            get => (DockTarget)GetValue(TargetDockProperty);
            set => SetValue(TargetDockProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public Brush GlyphBorderBrush
        {
            get => (Brush)GetValue(GlyphBorderBrushProperty);
            set => SetValue(GlyphBorderBrushProperty, value);
        }

        public Brush GlyphBackground
        {
            get => (Brush)GetValue(GlyphBackgroundProperty);
            set => SetValue(GlyphBackgroundProperty, value);
        }

        public Brush GlyphArrowBrush
        {
            get => (Brush)GetValue(GlyphArrowBrushProperty);
            set => SetValue(GlyphArrowBrushProperty, value);
        }

        public bool IsTargeted => (bool)GetValue(IsTargetedProperty);

        public object TargetHost
        {
            get => GetValue(TargetHostProperty);
            set => SetValue(TargetHostProperty, value);
        }

        public static object CoerceIsTargeted(DependencyObject d, object baseValue)
        {
            var btn = d as DockTargetButton;
            if (btn.Visibility != Visibility.Visible)
                return false;

            var pos = btn.PointFromScreen(NativeMethods.GetMousePosition());
            return new Rect(new Point(), btn.RenderSize).Contains(pos);
        }

        public DockTargetButton()
        {
            Loaded += DockTargetButton_Loaded;
            Unloaded += DockTargetButton_Unloaded;
        }

        private void DockTargetButton_Loaded(object sender, RoutedEventArgs e) => Instances.Add(this);
        private void DockTargetButton_Unloaded(object sender, RoutedEventArgs e) => Instances.Remove(this);
    }
}
