using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Studio.Controls
{
    public class SplitContainer : Control
    {
        private static readonly object splitterStyleKey = new Guid("af6c117f-3b20-4be8-84e1-c9b71a190a90");
        public static object SplitterStyleKey => splitterStyleKey;

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(SplitContainer), new PropertyMetadata(Orientation.Horizontal));

        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(nameof(ContentTemplate), typeof(DataTemplate), typeof(SplitContainer), new PropertyMetadata((DataTemplate)null));

        public static readonly DependencyProperty ContentTemplateSelectorProperty =
            DependencyProperty.Register(nameof(ContentTemplateSelector), typeof(DataTemplateSelector), typeof(SplitContainer), new PropertyMetadata((DataTemplateSelector)null));

        public static readonly DependencyProperty Panel1ContentProperty =
            DependencyProperty.Register(nameof(Panel1Content), typeof(object), typeof(SplitContainer), new PropertyMetadata(null));

        public static readonly DependencyProperty Panel2ContentProperty =
           DependencyProperty.Register(nameof(Panel2Content), typeof(object), typeof(SplitContainer), new PropertyMetadata(null));

        public static readonly DependencyProperty Panel1MinSizeProperty =
            DependencyProperty.Register(nameof(Panel1MinSize), typeof(double), typeof(SplitContainer), new PropertyMetadata(0d));

        public static readonly DependencyProperty Panel1SizeProperty =
            DependencyProperty.Register(nameof(Panel1Size), typeof(GridLength), typeof(SplitContainer), new PropertyMetadata(new GridLength(1, GridUnitType.Star)));

        public static readonly DependencyProperty Panel1MaxSizeProperty =
            DependencyProperty.Register(nameof(Panel1MaxSize), typeof(double), typeof(SplitContainer), new PropertyMetadata(double.PositiveInfinity));

        public static readonly DependencyProperty Panel2MinSizeProperty =
            DependencyProperty.Register(nameof(Panel2MinSize), typeof(double), typeof(SplitContainer), new PropertyMetadata(0d));

        public static readonly DependencyProperty Panel2SizeProperty =
            DependencyProperty.Register(nameof(Panel2Size), typeof(GridLength), typeof(SplitContainer), new PropertyMetadata(new GridLength(1, GridUnitType.Star)));

        public static readonly DependencyProperty Panel2MaxSizeProperty =
            DependencyProperty.Register(nameof(Panel2MaxSize), typeof(double), typeof(SplitContainer), new PropertyMetadata(double.PositiveInfinity));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        public DataTemplateSelector ContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
            set { SetValue(ContentTemplateSelectorProperty, value); }
        }

        public object Panel1Content
        {
            get { return (object)GetValue(Panel1ContentProperty); }
            set { SetValue(Panel1ContentProperty, value); }
        }

        public object Panel2Content
        {
            get { return (object)GetValue(Panel2ContentProperty); }
            set { SetValue(Panel2ContentProperty, value); }
        }

        public double Panel1MinSize
        {
            get { return (double)GetValue(Panel1MinSizeProperty); }
            set { SetValue(Panel1MinSizeProperty, value); }
        }

        public GridLength Panel1Size
        {
            get { return (GridLength)GetValue(Panel1SizeProperty); }
            set { SetValue(Panel1SizeProperty, value); }
        }

        public double Panel1MaxSize
        {
            get { return (double)GetValue(Panel1MaxSizeProperty); }
            set { SetValue(Panel1MaxSizeProperty, value); }
        }

        public double Panel2MinSize
        {
            get { return (double)GetValue(Panel2MinSizeProperty); }
            set { SetValue(Panel2MinSizeProperty, value); }
        }

        public GridLength Panel2Size
        {
            get { return (GridLength)GetValue(Panel2SizeProperty); }
            set { SetValue(Panel2SizeProperty, value); }
        }

        public double Panel2MaxSize
        {
            get { return (double)GetValue(Panel2MaxSizeProperty); }
            set { SetValue(Panel2MaxSizeProperty, value); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static SplitContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitContainer), new FrameworkPropertyMetadata(typeof(SplitContainer)));
        }
    }
}
