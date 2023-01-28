using System.Windows;

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
            get => GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        public RaftedWindow()
        {
            InitializeComponent();
        }
    }
}
