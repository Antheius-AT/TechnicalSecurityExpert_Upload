//-----------------------------------------------------------------------
// <copyright file="SignalRClient.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.Models
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR.Client;
    using SharedData.SharedHubData.Interfaces;

    /// <summary>
    /// Represents a SignalR implementation of the client contract interface.
    /// </summary>
    public class SignalRClient : IClientContract
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignalRClient"/> class.
        /// </summary>
        /// <param name="name">The client name.</param>
        /// <param name="hubConnection">The hub connection.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either of the parameters are null.
        /// </exception>
        public SignalRClient(string name, HubConnection hubConnection)
        {
            this.ClientName = name;
            this.HubConnection = hubConnection;
        }

        /// <summary>
        /// Gets the client username.
        /// </summary>
        public string ClientName
        {
            get;
        }

        /// <summary>
        /// Gets the hub connection.
        /// </summary>
        public HubConnection HubConnection
        {
            get;
        }

        /// <summary>
        /// Sends a message asynchronously to the server.
        /// </summary>
        /// <param name="methodName">The method to invoke on the server.</param>
        /// <param name="formattedMessage">The formatted message data.</param>
        /// <returns>A task object.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either of the parameters are null.
        /// </exception>
        public async Task SendMessageAsync(string methodName, string formattedMessage)
        {
            await this.HubConnection.SendAsync(methodName, formattedMessage);
        }

        /// <summary>
        /// Tries to connect to a remote endpoint asynchronously.
        /// </summary>
        /// <returns>A task object handling the logic to connect.</returns>
        public async Task<bool> TryConnectAsync()
        {
            try
            {
                await this.HubConnection.StartAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
