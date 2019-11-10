using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sandbox.Controls
{
    /// <summary>
    /// Interaction logic for RaftedWindow.xaml
    /// </summary>
    public partial class RaftedWindow : MahApps.Metro.Controls.MetroWindow
    {
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register(nameof(Model), typeof(object), typeof(RaftedWindow), new PropertyMetadata((object)null));

        public object Model
        {
            get { return (object)GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        public RaftedWindow()
        {
            InitializeComponent();
        }
    }
}
