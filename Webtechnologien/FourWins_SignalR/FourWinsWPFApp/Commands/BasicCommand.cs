using System;
using System.Windows.Input;

namespace FourWinsWPFApp.Commands
{
    public class BasicCommand : ICommand
    {
        private Action<object> action;

        public event EventHandler CanExecuteChanged;

        public BasicCommand(Action<object> action)
        {
            this.action = action;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.action(parameter);
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        protected virtual void RaisePropertyChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
