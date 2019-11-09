using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Studio.Controls
{
    public class DocumentContainer : ItemsControl
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static DocumentContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DocumentContainer), new FrameworkPropertyMetadata(typeof(DocumentContainer)));
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(DocumentContainer), new PropertyMetadata(Orientation.Horizontal));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DocumentWell();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DocumentWell;
        }
    }
}
