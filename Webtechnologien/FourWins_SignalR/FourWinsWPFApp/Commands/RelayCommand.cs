//-----------------------------------------------------------------------
// <copyright file="MainWindowXaml.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
using System;
using System.Windows.Input;

namespace FourWinsWPFApp
{
    /// <summary>
    /// Represents a command which invokes actions that are injected into its constructor.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">An anonymous function executing the command.</param>
        /// <param name="canExecute">An anonymous function returning whether this command can execute.</param>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Notifies subscribers that the status of whether can execute changed has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// This method evaluates whether the command can execute.
        /// </summary>
        /// <param name="parameter">A parameter used for determining executability.</param>
        /// <returns>Whether the command can execute.</returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecute(parameter);
        }

        /// <summary>
        /// This method invokes the command.
        /// </summary>
        /// <param name="parameter">The parameter passed to the command on invocation.</param>
        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }
}
