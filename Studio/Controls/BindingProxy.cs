using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Studio.Controls
{
    public class BindingProxy : Freezable
    {
        public static readonly DependencyProperty ContextProperty =
            DependencyProperty.Register(nameof(Context), typeof(object), typeof(BindingProxy), new PropertyMetadata(null));

        public object Context
        {
            get { return GetValue(ContextProperty); }
            set { SetValue(ContextProperty, value); }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
    }
}
