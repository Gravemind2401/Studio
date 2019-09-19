using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Studio.Controls
{
    public class ToolWell : TabWellBase
    {
        private static readonly object tabStyleKey = new Guid("6bed4f5f-4da7-4c65-8efa-a5b7b999885a");
        public static object ToolTabStyleKey => tabStyleKey;

        public static readonly DependencyProperty TogglePinStatusCommandProperty =
            DependencyProperty.Register(nameof(TogglePinStatusCommand), typeof(ICommand), typeof(ToolWell), new PropertyMetadata(Commands.PinToolCommand));

        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register(nameof(CloseCommand), typeof(ICommand), typeof(ToolWell), new PropertyMetadata(Commands.CloseToolCommand));

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register(nameof(Caption), typeof(object), typeof(ToolWell), new PropertyMetadata(null));

        public static readonly DependencyProperty ShowCaptionProperty =
            DependencyProperty.Register(nameof(ShowCaption), typeof(bool), typeof(ToolWell), new PropertyMetadata(true));

        public ICommand TogglePinStatusCommand
        {
            get { return (ICommand)GetValue(TogglePinStatusCommandProperty); }
            set { SetValue(TogglePinStatusCommandProperty, value); }
        }

        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ToolWell()
        {
            TabStripPlacementProperty.OverrideMetadata(typeof(ToolWell), new FrameworkPropertyMetadata(Dock.Bottom));
        }

        public ToolWell()
        {
            CommandBindings.Add(new CommandBinding(Commands.PinToolCommand, PinToolCommandExecuted));
            CommandBindings.Add(new CommandBinding(Commands.CloseToolCommand, CloseToolCommandExecuted));
        }

        #region Command Handlers
        private void PinToolCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void CloseToolCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Items.Remove(SelectedItem);
        } 
        #endregion
    }
}
