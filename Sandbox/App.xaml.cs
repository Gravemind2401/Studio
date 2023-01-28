using System.Windows;

namespace Sandbox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static App Instance { get; private set; }

        public App() : base()
        {
            Instance = this;
        }
    }
}
