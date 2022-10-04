//-----------------------------------------------------------------------
// <copyright file="ILoginVM.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.Interfaces
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    /// <summary>
    /// Represents an object capable of binding to login views.
    /// </summary>
    public interface ILoginVM<TErrorEventArgs> : INotifyPropertyChanged where TErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the command which executes login logic.
        /// </summary>
        ICommand LoginCommand { get; }

        /// <summary>
        /// Notifies subscribing views that a closing of the view is requested.
        /// </summary>
        event EventHandler ClosingRequested;

        /// <summary>
        /// Notifies subscribing views that an error occurred.
        /// </summary>
        event EventHandler<TErrorEventArgs> ErrorOccurred;

        event EventHandler<CreateWindowsEventArgs> ConnectionSuccessful_CreateWindows;
    }
}
