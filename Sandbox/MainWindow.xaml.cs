using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sandbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Dictionary<string, ResourceDictionary> themes = (new[]
        {
            "Blue", "Dark", "Light", "Green", "Purple", "Red", "Tan", "Solarized (Dark)", "Solarized (Light)"
        }).ToDictionary(s => s, s => new ResourceDictionary { Source = new Uri($"/Studio;component/Themes/{Regex.Replace(s, @"[ \(\)]", string.Empty)}.xaml", UriKind.RelativeOrAbsolute) });

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var theme in themes)
            {
                var item = new MenuItem { Header = theme.Key };
                item.Click += (s, args) =>
                {
                    App.Instance.Resources.MergedDictionaries.Clear();
                    App.Instance.Resources.MergedDictionaries.Add(theme.Value);
                };
                ThemesMenu.Items.Add(item);
            }

            var none = new MenuItem { Header = "(None)" };
            none.Click += (s, args) => App.Instance.Resources.MergedDictionaries.Clear();
            ThemesMenu.Items.Add(none);
        }
    }
}
