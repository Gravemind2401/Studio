﻿using System.Windows;
using System.Windows.Data;

namespace Studio.Utilities
{
    public class ProxyBinding : Freezable
    {
        private const string ProxyBindingsPropertyName = "ProxyBindings";

        public static readonly DependencyProperty ProxyBindingsProperty =
            DependencyProperty.RegisterAttached(ProxyBindingsPropertyName, typeof(ProxyBindingCollection), typeof(ProxyBinding), new PropertyMetadata((ProxyBindingCollection)null));

        public static ProxyBindingCollection GetProxyBindings(DependencyObject obj) => (ProxyBindingCollection)obj.GetValue(ProxyBindingsProperty);
        public static void SetProxyBindings(DependencyObject obj, ProxyBindingCollection value) => obj.SetValue(ProxyBindingsProperty, value);

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(object), typeof(ProxyBinding), new PropertyMetadata(null, SourcePropertyChanged));

        public object Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private static void SourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //only propagate changes if the target is actually bound to something
            if (BindingOperations.GetBindingBase(d, TargetProperty) != null)
                (d as ProxyBinding).Target = e.NewValue;
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register(nameof(Target), typeof(object), typeof(ProxyBinding), new PropertyMetadata((object)null));

        public object Target
        {
            get => GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        protected override Freezable CreateInstanceCore() => new ProxyBinding();
    }

    public class ProxyBindingCollection : FreezableCollection<ProxyBinding>
    {

    }
}
