using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Studio.Controls
{
    [TemplatePart(Name = PART_AcceptButton, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = PART_CancelButton, Type = typeof(FrameworkElement))]
    [StyleTypedProperty(Property = nameof(AcceptButtonStyle), StyleTargetType = typeof(Button))]
    [StyleTypedProperty(Property = nameof(CancelButtonStyle), StyleTargetType = typeof(Button))]
    public class SearchControl : TextBox
    {
        private const string PART_AcceptButton = "PART_AcceptButton";
        private const string PART_CancelButton = "PART_CancelButton";

        private const int DefaultSearchDelay = 300;
        private readonly DispatcherTimer SearchTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(DefaultSearchDelay) };

        private Button AcceptButton;
        private Button CancelButton;
        private string LastSearch;

        static SearchControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchControl), new FrameworkPropertyMetadata(typeof(SearchControl)));
        }

        #region Routed Events
        public static readonly RoutedEvent SearchAcceptedEvent =
            EventManager.RegisterRoutedEvent(nameof(SearchAccepted), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchControl));

        public static readonly RoutedEvent SearchCanceledEvent =
            EventManager.RegisterRoutedEvent(nameof(SearchCanceled), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SearchControl));

        public event RoutedEventHandler SearchAccepted
        {
            add { AddHandler(SearchAcceptedEvent, value); }
            remove { RemoveHandler(SearchAcceptedEvent, value); }
        }

        public event RoutedEventHandler SearchCanceled
        {
            add { AddHandler(SearchCanceledEvent, value); }
            remove { RemoveHandler(SearchCanceledEvent, value); }
        }
        #endregion

        #region Dependency Properties
        private static readonly DependencyPropertyKey HasTextPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(nameof(HasText), typeof(bool), typeof(SearchControl), new PropertyMetadata(false, null, CoerceHasText));

        public static readonly DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;

        public static readonly DependencyProperty AcceptButtonStyleProperty =
            DependencyProperty.Register(nameof(AcceptButtonStyle), typeof(Style), typeof(SearchControl));

        public static readonly DependencyProperty CancelButtonStyleProperty =
            DependencyProperty.Register(nameof(CancelButtonStyle), typeof(Style), typeof(SearchControl));

        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.Register(nameof(WatermarkText), typeof(string), typeof(SearchControl), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsSearchActiveProperty =
            DependencyProperty.Register(nameof(IsSearchActive), typeof(bool), typeof(SearchControl), new PropertyMetadata(false));

        public static readonly DependencyProperty LiveSearchEnabledProperty =
            DependencyProperty.Register(nameof(LiveSearchEnabled), typeof(bool), typeof(SearchControl), new PropertyMetadata(false));

        public static readonly DependencyProperty LiveSearchDelayProperty =
            DependencyProperty.Register(nameof(LiveSearchDelay), typeof(int), typeof(SearchControl), new PropertyMetadata(DefaultSearchDelay, LiveSearchTimeoutChanged), ValidateLiveSearchDelay);

        public bool HasText => (bool)GetValue(HasTextProperty);

        public Style AcceptButtonStyle
        {
            get => (Style)GetValue(AcceptButtonStyleProperty);
            set => SetValue(AcceptButtonStyleProperty, value);
        }

        public Style CancelButtonStyle
        {
            get => (Style)GetValue(CancelButtonStyleProperty);
            set => SetValue(CancelButtonStyleProperty, value);
        }

        public string WatermarkText
        {
            get => (string)GetValue(WatermarkTextProperty);
            set => SetValue(WatermarkTextProperty, value);
        }

        public bool IsSearchActive
        {
            get => (bool)GetValue(IsSearchActiveProperty);
            set => SetValue(IsSearchActiveProperty, value);
        }

        public bool LiveSearchEnabled
        {
            get => (bool)GetValue(LiveSearchEnabledProperty);
            set => SetValue(LiveSearchEnabledProperty, value);
        }

        public int LiveSearchDelay
        {
            get => (int)GetValue(LiveSearchDelayProperty);
            set => SetValue(LiveSearchDelayProperty, value);
        }

        public static object CoerceHasText(DependencyObject obj, object baseValue) => !string.IsNullOrEmpty((obj as SearchControl)?.Text);

        public static bool ValidateLiveSearchDelay(object value) => value is int i && i >= 0;

        public static void LiveSearchTimeoutChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var searchBox = obj as SearchControl;
            searchBox.SearchTimer.Interval = TimeSpan.FromMilliseconds((int)e.NewValue);
        }
        #endregion

        public SearchControl()
        {
            Loaded += SearchControl_Loaded;
        }

        private void SearchControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= SearchControl_Loaded;
            Unloaded += SearchControl_Unloaded;

            SearchTimer.Tick += SearchTimer_Tick;
            TextChanged += SearchControl_TextChanged;
            CoerceValue(HasTextProperty);
        }

        private void SearchControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= SearchControl_Unloaded;
            Loaded += SearchControl_Loaded;

            SearchTimer.Tick -= SearchTimer_Tick;
            TextChanged -= SearchControl_TextChanged;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            OnTemplateUnset();
            AcceptButton = Template.FindName(PART_AcceptButton, this) as Button;
            CancelButton = Template.FindName(PART_CancelButton, this) as Button;
            OnTemplateSet();
        }

        private void OnTemplateUnset()
        {
            if (AcceptButton != null)
                AcceptButton.Click -= AcceptButton_Click;

            if (CancelButton != null)
                CancelButton.Click -= CancelButton_Click;
        }

        private void OnTemplateSet()
        {
            if (AcceptButton != null)
                AcceptButton.Click += AcceptButton_Click;

            if (CancelButton != null)
                CancelButton.Click += CancelButton_Click;
        }

        private void StartSearch()
        {
            SearchTimer.Stop();

            if (string.Equals(Text, LastSearch, StringComparison.Ordinal))
                return;

            LastSearch = Text;
            IsSearchActive = true;
            RaiseEvent(new RoutedEventArgs(SearchAcceptedEvent, this));
        }

        private void StopSearch()
        {
            SearchTimer.Stop();

            if (!IsSearchActive)
                return;

            LastSearch = null;
            IsSearchActive = false;
            Clear();
            RaiseEvent(new RoutedEventArgs(SearchCanceledEvent, this));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Handled)
                return;

            if (e.Key == Key.Escape || (string.IsNullOrEmpty(Text) && e.Key == Key.Enter))
                StopSearch();
            else if (e.Key == Key.Enter)
                StartSearch();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (e.Handled)
                return;

            if (!string.IsNullOrEmpty(Text))
                StartSearch();
            else
                StopSearch();
        }

        private void SearchControl_TextChanged(object sender, TextChangedEventArgs e)
        {
            CoerceValue(HasTextProperty);
            SearchTimer.Stop();

            if (LiveSearchEnabled)
            {
                if (LiveSearchDelay == 0)
                    SearchTimer_Tick(null, null);
                else
                    SearchTimer.Start();
            }
        }

        private void SearchTimer_Tick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
                StopSearch();
            else
                StartSearch();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Text))
                StopSearch();
            else
                StartSearch();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => StopSearch();
    }
}
