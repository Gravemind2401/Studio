using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Studio.Controls
{
    public class DockContainer : ContentControl
    {
        #region Dependency Properties
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(DockContainer), new PropertyMetadata((DataTemplate)null));

        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register(nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(DockContainer), new PropertyMetadata((DataTemplateSelector)null));

        public static readonly DependencyProperty LeftItemsSourceProperty =
            DependencyProperty.Register(nameof(LeftItemsSource), typeof(IEnumerable), typeof(DockContainer), new PropertyMetadata((IEnumerable)null));

        public static readonly DependencyProperty TopItemsSourceProperty =
            DependencyProperty.Register(nameof(TopItemsSource), typeof(IEnumerable), typeof(DockContainer), new PropertyMetadata((IEnumerable)null));

        public static readonly DependencyProperty RightItemsSourceProperty =
            DependencyProperty.Register(nameof(RightItemsSource), typeof(IEnumerable), typeof(DockContainer), new PropertyMetadata((IEnumerable)null));

        public static readonly DependencyProperty BottomItemsSourceProperty =
            DependencyProperty.Register(nameof(BottomItemsSource), typeof(IEnumerable), typeof(DockContainer), new PropertyMetadata((IEnumerable)null));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        public IEnumerable LeftItemsSource
        {
            get { return (IEnumerable)GetValue(LeftItemsSourceProperty); }
            set { SetValue(LeftItemsSourceProperty, value); }
        }

        public IEnumerable TopItemsSource
        {
            get { return (IEnumerable)GetValue(TopItemsSourceProperty); }
            set { SetValue(TopItemsSourceProperty, value); }
        }

        public IEnumerable RightItemsSource
        {
            get { return (IEnumerable)GetValue(RightItemsSourceProperty); }
            set { SetValue(RightItemsSourceProperty, value); }
        }

        public IEnumerable BottomItemsSource
        {
            get { return (IEnumerable)GetValue(BottomItemsSourceProperty); }
            set { SetValue(BottomItemsSourceProperty, value); }
        } 
        #endregion

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static DockContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockContainer), new FrameworkPropertyMetadata(typeof(DockContainer)));
        }
    }
}
