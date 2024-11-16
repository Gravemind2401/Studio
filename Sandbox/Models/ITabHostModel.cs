using Prism.Commands;

namespace Sandbox.Models
{
    public interface ITabHostModel
    {
        DelegateCommand<TabModel> CloseTabCommand { get; }
        DelegateCommand<TabModel> TogglePinStatusCommand { get; }
    }
}
