﻿using System.Windows;
using System.Windows.Input;

namespace Studio.Controls
{
    public static class Commands
    {
        public static readonly RoutedCommand PinTabCommand = new RoutedCommand(nameof(PinTabCommand), typeof(ICommandSource));
        public static readonly RoutedCommand CloseTabCommand = new RoutedCommand(nameof(CloseTabCommand), typeof(ICommandSource));
        public static readonly RoutedCommand SelectTabCommand = new RoutedCommand(nameof(SelectTabCommand), typeof(ICommandSource));

        public static readonly RoutedCommand CloseToolCommand = new RoutedCommand(nameof(CloseToolCommand), typeof(ICommandSource));

        internal static readonly RoutedCommand TabMouseDownCommand = new RoutedCommand(nameof(TabMouseDownCommand), typeof(ICommandSource));
        internal static readonly RoutedCommand TabMouseMoveCommand = new RoutedCommand(nameof(TabMouseMoveCommand), typeof(ICommandSource));
        internal static readonly RoutedCommand TabMouseUpCommand = new RoutedCommand(nameof(TabMouseUpCommand), typeof(ICommandSource));
        internal static readonly RoutedCommand TabLostMouseCaptureCommand = new RoutedCommand(nameof(TabLostMouseCaptureCommand), typeof(ICommandSource));

        internal static void TryExecute(this ICommand command, object parameter) => TryExecute(command, parameter, null);

        internal static void TryExecute(this ICommand command, object parameter, IInputElement target)
        {
            if (command == null)
                return;

            if (command is RoutedCommand routed)
            {
                if (routed.CanExecute(parameter, target))
                    routed.Execute(parameter, target);
            }
            else if (command.CanExecute(parameter))
                command.Execute(parameter);
        }
    }
}
