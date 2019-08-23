using System;
using System.Windows.Input;

namespace FileEncryptorWpf.ViewModels
{
    /// <summary>
    /// Class used for delegating commands to View in MVVM architecture without arguments.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Action action;

        public DelegateCommand(Action action)
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
            this.action();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }
    }
}