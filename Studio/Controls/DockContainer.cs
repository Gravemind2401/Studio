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
    [TemplatePart(Name = nameof(PART_ContentHost), Type = typeof(ContentPresenter))]
    [StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(ListBoxItem))]
    public class DockContainer : ContentControl
    {
        private const string PART_ContentHost = "PART_ContentHost";

        private ContentPresenter contentHost;

        #region Dependency Properties
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register(nameof(ItemContainerStyle), typeof(Style), typeof(DockContainer), new PropertyMetadata((Style)null));

        public static readonly DependencyProperty ItemContainerStyleSelectorProperty =
            DependencyProperty.Register(nameof(ItemContainerStyleSelector), typeof(StyleSelector), typeof(DockContainer), new PropertyMetadata((StyleSelector)null));

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

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(DockContainer), new PropertyMetadata(null, SelectedItemChanged));

        private static void SelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (DockContainer)d;
            c.CoerceValue(HasSelectionProperty);
            c.CoerceValue(SelectedItemDockProperty);
        }

        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(nameof(SelectedValue), typeof(object), typeof(DockContainer), new PropertyMetadata((object)null));

        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register(nameof(SelectedValuePath), typeof(string), typeof(DockContainer), new PropertyMetadata((object)null));

        public static readonly DependencyProperty SelectedContentTemplateProperty =
            DependencyProperty.Register(nameof(SelectedContentTemplate), typeof(DataTemplate), typeof(DockContainer), new PropertyMetadata((DataTemplate)null));

        public static readonly DependencyProperty SelectedContentTemplateSelectorProperty =
            DependencyProperty.Register(nameof(SelectedContentTemplateSelector), typeof(DataTemplateSelector), typeof(DockContainer), new PropertyMetadata((DataTemplateSelector)null));

        public static readonly DependencyPropertyKey HasSelectionPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(HasSelection), typeof(bool), typeof(DockContainer), new PropertyMetadata(false, null, CoerceHasSelection));

        public static readonly DependencyProperty HasSelectionProperty = HasSelectionPropertyKey.DependencyProperty;

        private static object CoerceHasSelection(DependencyObject d, object baseValue)
        {
            return ((DockContainer)d).SelectedItem != null;
        }

        public static readonly DependencyPropertyKey SelectedItemDockPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(SelectedItemDock), typeof(Dock?), typeof(DockContainer), new PropertyMetadata(null, null, CoerceSelectedItemDock));

        public static readonly DependencyProperty SelectedItemDockProperty = SelectedItemDockPropertyKey.DependencyProperty;

        private static object CoerceSelectedItemDock(DependencyObject d, object baseValue)
        {
            var c = (DockContainer)d;
            if (c.SelectedItem == null)
                return null;
            else if (c.LeftItemsSource.OfType<object>().Contains(c.SelectedItem))
                return Dock.Left;
            else if (c.RightItemsSource.OfType<object>().Contains(c.SelectedItem))
                return Dock.Right;
            else if (c.BottomItemsSource.OfType<object>().Contains(c.SelectedItem))
                return Dock.Bottom;
            else if (c.TopItemsSource.OfType<object>().Contains(c.SelectedItem))
                return Dock.Top;
            else return null;
        }

        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        public StyleSelector ItemContainerStyleSelector
        {
            get { return (StyleSelector)GetValue(ItemContainerStyleSelectorProperty); }
            set { SetValue(ItemContainerStyleSelectorProperty, value); }
        }

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

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public object SelectedValue
        {
            get { return GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        public string SelectedValuePath
        {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        public DataTemplate SelectedContentTemplate
        {
            get { return (DataTemplate)GetValue(SelectedContentTemplateProperty); }
            set { SetValue(SelectedContentTemplateProperty, value); }
        }

        public DataTemplateSelector SelectedContentTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(SelectedContentTemplateSelectorProperty); }
            set { SetValue(SelectedContentTemplateSelectorProperty, value); }
        }

        public bool HasSelection
        {
            get { return (bool)GetValue(HasSelectionProperty); }
        }

        public Dock? SelectedItemDock
        {
            get { return (Dock?)GetValue(SelectedItemDockProperty); }
        }
        #endregion

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static DockContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockContainer), new FrameworkPropertyMetadata(typeof(DockContainer)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (contentHost != null)
                contentHost.PreviewMouseDown -= ContentHost_PreviewMouseDown;

            contentHost = Template.FindName(PART_ContentHost, this) as ContentPresenter;

            if (contentHost != null)
                contentHost.PreviewMouseDown += ContentHost_PreviewMouseDown;
        }

        private void ContentHost_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SelectedItem = null;
        }
    }
}
