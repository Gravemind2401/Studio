using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Studio.Controls
{
    [TemplatePart(Name = nameof(PART_ContentHost), Type = typeof(ContentPresenter))]
    [StyleTypedProperty(Property = nameof(ItemContainerStyle), StyleTargetType = typeof(ListBoxItem))]
    public class DockContainer : ContentControl, IDockReceiver
    {
        private const string PART_ContentHost = "PART_ContentHost";

        internal ContentPresenter ContentHost { get; private set; }

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
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(DockContainer), new FrameworkPropertyMetadata(null, SelectedItemChanged) { BindsTwoWayByDefault = true });

        private static void SelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (DockContainer)d;
            c.CoerceValue(HasSelectionProperty);
            c.CoerceValue(SelectedItemDockProperty);
        }

        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(nameof(SelectedValue), typeof(object), typeof(DockContainer), new FrameworkPropertyMetadata((object)null) { BindsTwoWayByDefault = true });

        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register(nameof(SelectedValuePath), typeof(string), typeof(DockContainer), new PropertyMetadata((object)null));

        public static readonly DependencyProperty SelectedContentTemplateProperty =
            DependencyProperty.Register(nameof(SelectedContentTemplate), typeof(DataTemplate), typeof(DockContainer), new PropertyMetadata((DataTemplate)null));

        public static readonly DependencyProperty SelectedContentTemplateSelectorProperty =
            DependencyProperty.Register(nameof(SelectedContentTemplateSelector), typeof(DataTemplateSelector), typeof(DockContainer), new PropertyMetadata((DataTemplateSelector)null));

        private static readonly DependencyPropertyKey HasSelectionPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(HasSelection), typeof(bool), typeof(DockContainer), new PropertyMetadata(false, null, CoerceHasSelection));

        public static readonly DependencyProperty HasSelectionProperty = HasSelectionPropertyKey.DependencyProperty;

        private static object CoerceHasSelection(DependencyObject d, object baseValue) => ((DockContainer)d).SelectedItem != null;

        private static readonly DependencyPropertyKey SelectedItemDockPropertyKey =
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

            return null;
        }

        public static readonly DependencyProperty DockCommandProperty =
            DependencyProperty.Register(nameof(DockCommand), typeof(ICommand), typeof(DockContainer), new PropertyMetadata((ICommand)null));

        public ICommand DockCommand
        {
            get => (ICommand)GetValue(DockCommandProperty);
            set => SetValue(DockCommandProperty, value);
        }

        public Style ItemContainerStyle
        {
            get => (Style)GetValue(ItemContainerStyleProperty);
            set => SetValue(ItemContainerStyleProperty, value);
        }

        public StyleSelector ItemContainerStyleSelector
        {
            get => (StyleSelector)GetValue(ItemContainerStyleSelectorProperty);
            set => SetValue(ItemContainerStyleSelectorProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public DataTemplateSelector ItemTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty);
            set => SetValue(ItemTemplateSelectorProperty, value);
        }

        public IEnumerable LeftItemsSource
        {
            get => (IEnumerable)GetValue(LeftItemsSourceProperty);
            set => SetValue(LeftItemsSourceProperty, value);
        }

        public IEnumerable TopItemsSource
        {
            get => (IEnumerable)GetValue(TopItemsSourceProperty);
            set => SetValue(TopItemsSourceProperty, value);
        }

        public IEnumerable RightItemsSource
        {
            get => (IEnumerable)GetValue(RightItemsSourceProperty);
            set => SetValue(RightItemsSourceProperty, value);
        }

        public IEnumerable BottomItemsSource
        {
            get => (IEnumerable)GetValue(BottomItemsSourceProperty);
            set => SetValue(BottomItemsSourceProperty, value);
        }

        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public object SelectedValue
        {
            get => GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        public string SelectedValuePath
        {
            get => (string)GetValue(SelectedValuePathProperty);
            set => SetValue(SelectedValuePathProperty, value);
        }

        public DataTemplate SelectedContentTemplate
        {
            get => (DataTemplate)GetValue(SelectedContentTemplateProperty);
            set => SetValue(SelectedContentTemplateProperty, value);
        }

        public DataTemplateSelector SelectedContentTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(SelectedContentTemplateSelectorProperty);
            set => SetValue(SelectedContentTemplateSelectorProperty, value);
        }

        public bool HasSelection => (bool)GetValue(HasSelectionProperty);

        public Dock? SelectedItemDock => (Dock?)GetValue(SelectedItemDockProperty);
        #endregion

        static DockContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockContainer), new FrameworkPropertyMetadata(typeof(DockContainer)));
        }

        public DockContainer()
        {
            Loaded += DockContainer_Loaded;
            Unloaded += DockContainer_Unloaded;
        }

        private void DockContainer_Loaded(object sender, RoutedEventArgs e) => DockManager.Register(this);
        private void DockContainer_Unloaded(object sender, RoutedEventArgs e) => DockManager.Unregister(this);

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ContentHost != null)
                ContentHost.PreviewMouseDown -= ContentHost_PreviewMouseDown;

            ContentHost = Template.FindName(PART_ContentHost, this) as ContentPresenter;

            if (ContentHost != null)
                ContentHost.PreviewMouseDown += ContentHost_PreviewMouseDown;
        }

        private void ContentHost_PreviewMouseDown(object sender, MouseButtonEventArgs e) => SelectedItem = null;
    }
}
