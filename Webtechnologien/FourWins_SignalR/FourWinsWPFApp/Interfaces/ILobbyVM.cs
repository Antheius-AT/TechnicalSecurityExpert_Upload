//-----------------------------------------------------------------------
// <copyright file="ILobbyVM.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman, Tamara Mayer</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.Interfaces
{
    using System;
    using System.ComponentModel;
    using FourWinsWPFApp.EventArguments;
    using FourWinsWPFApp.Vms;
    using SharedData.SharedHubData.Interfaces;

    /// <summary>
    /// Represents a view model that lobby views can bind to.
    /// </summary>
    public interface ILobbyVM : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets the client for this view model.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if you attempt to set null.
        /// </exception>
        IClientContract Client { get; }

        /// <summary>
        /// Notifies subscribers that a connection failed due to an invalid username.
        /// </summary>
        event EventHandler<InvalidUsernameEventArguments> InvalidUsernameConnectionFailed;

        /// <summary>
        /// Notifies subscribers that the connection was closed.
        /// </summary>
        event EventHandler<ConnectionClosedEventArguments> ConnectionClosed;

        /// <summary>
        /// Notifies subscribers that a new game is to be created.
        /// </summary>
        public event EventHandler<GameCreationRequestedEventArgs> GameCreationRequested;

        /// <summary>
        /// Notifies subscribers that a new game for watching shall be requested.
        /// </summary>
        public event EventHandler<WatchingGameRequestedEventArgs> WatchingGameRequested;
    }
}
