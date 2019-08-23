using System;
using System.Windows.Input;

namespace FileEncryptorWpf.ViewModels
{
    /// <summary>
    /// Class used for delegating commands to View in MVVM architecture with arguments.
    /// </summary>
    public class DelegateCommandWithParameter : ICommand
    {
        private readonly Action<object> action;

        public DelegateCommandWithParameter(Action<object> action)
        {
            this.action = action;
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }

        public void Execute(object parameter)
        {
            this.action(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
    }
}