using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Studio.Utilities
{
    public class ProxyBinding : Freezable
    {
        public static readonly DependencyProperty ProxyBindingsProperty =
            DependencyProperty.RegisterAttached("ProxyBindings", typeof(ProxyBindingCollection), typeof(ProxyBinding), new PropertyMetadata((ProxyBindingCollection)null));

        public static ProxyBindingCollection GetProxyBindings(DependencyObject obj)
        {
            return (ProxyBindingCollection)obj.GetValue(ProxyBindingsProperty);
        }

        public static void SetProxyBindings(DependencyObject obj, ProxyBindingCollection value)
        {
            obj.SetValue(ProxyBindingsProperty, value);
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(object), typeof(ProxyBinding), new PropertyMetadata(null, SourcePropertyChanged));

        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private static void SourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ProxyBinding).Target = e.NewValue;
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register(nameof(Target), typeof(object), typeof(ProxyBinding), new PropertyMetadata((object)null));

        public object Target
        {
            get { return GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new ProxyBinding();
        }
    }

    public class ProxyBindingCollection : FreezableCollection<ProxyBinding>
    {

    }
}
