using System;
using System.Windows.Input;

namespace SaveRedirection.ViewModels.Commands
{
    public class ItemCommand : ICommand
    {
        private readonly Action<Redirection> _execute;
        public ItemCommand(Action<Redirection> execute)
        {
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute.Invoke((Redirection)parameter);
        }
    }
}
