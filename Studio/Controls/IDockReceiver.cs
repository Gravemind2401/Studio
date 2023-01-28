using System.Windows.Input;

namespace Studio.Controls
{
    internal interface IDockReceiver
    {
        ICommand DockCommand { get; }
    }
}
