using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Studio.Controls
{
    public class ToolWell : TabWellBase
    {
        private static readonly object tabStyleKey = new Guid("6bed4f5f-4da7-4c65-8efa-a5b7b999885a");
        public static object ToolTabStyleKey => tabStyleKey;

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register(nameof(Caption), typeof(object), typeof(ToolWell), new PropertyMetadata(null));

        public static readonly DependencyProperty ShowCaptionProperty =
            DependencyProperty.Register(nameof(ShowCaption), typeof(bool), typeof(ToolWell), new PropertyMetadata(true));

        public object Caption
        {
            get { return (object)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public bool ShowCaption
        {
            get { return (bool)GetValue(ShowCaptionProperty); }
            set { SetValue(ShowCaptionProperty, value); }
        }
    }
}
