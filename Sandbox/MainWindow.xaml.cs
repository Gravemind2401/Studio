﻿using Sandbox.Controls;
using Sandbox.Models;
using Sandbox.ViewModels;
using Studio.Controls;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Sandbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        private static readonly Dictionary<string, ResourceDictionary> themes = (new[]
        {
            "Blue", "Dark", "Light", "Green", "Purple", "Red", "Tan", "Solarized (Dark)", "Solarized (Light)"
        }).ToDictionary(s => s, s => new ResourceDictionary { Source = new Uri($"/Studio;component/Themes/{Regex.Replace(s, @"[ \(\)]", string.Empty)}.xaml", UriKind.RelativeOrAbsolute) });

        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register(nameof(Model), typeof(object), typeof(MainWindow), new PropertyMetadata((object)null));

        public object Model
        {
            get => GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

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

            CreateModel();
        }

        private void CreateModel()
        {
            var model = new WindowViewModel();
            var item1 = new DocContainerModel(new DocumentWellModel() { IsActive = true });

            for (var i = 0; i < 5; i++)
            {
                item1.Children[0].Children.Add(new TabModel
                {
                    Header = $"Tab Item {i}",
                    Content = new DocumentControl()
                });
            }

            var content = new SplitViewModel
            {
                Item1 = item1,
                Item2 = new SplitViewModel
                {
                    Orientation = Orientation.Vertical,
                    Item1 = GenerateToolGroup(Dock.Right),
                    Item2 = GenerateToolGroup(Dock.Right)
                },
                Item2Size = new GridLength(WindowViewModel.DefaultDockSize)
            };

            model.Content = content;
            Model = model;
        }

        private static ToolWellModel GenerateToolGroup(Dock dock)
        {
            var item = new ToolWellModel() { Dock = dock };
            for (var i = 0; i < 3; i++)
            {
                item.Children.Add(new TabModel
                {
                    Header = $"Tool Item {i}",
                    ToolTip = $"Tool Item {i} Long Name",
                    Content = new ToolControl(),
                    Usage = TabItemType.Tool
                });
            }

            return item;
        }
    }
}
