//-----------------------------------------------------------------------
// <copyright file="ClientFactoryService" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.Services
{
    using System;
    using FourWinsWPFApp.Interfaces;
    using FourWinsWPFApp.Models;
    using Microsoft.AspNetCore.SignalR.Client;

    /// <summary>
    /// Represents a service handling login logic.
    /// </summary>
    public class ClientFactoryService : IClientFactoryService<SignalRClient>
    {
        /// <summary>
        /// Logs in to a remote endpoint using the specified connection information.
        /// </summary>
        /// <param name="connectionInfo">The specified connection information.</param>
        /// <returns>A task object returning a signalR client upon termination.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if connection info is null.
        /// </exception>
        public SignalRClient CreateClient(ConnectionInfo connectionInfo, string URLPostfix)
        {
            var hubConnection = new HubConnectionBuilder().WithUrl(string.Concat(connectionInfo.ServerURL, URLPostfix)).Build();
            var client = new SignalRClient(connectionInfo.Username, hubConnection);

            return client;
        }
    }
}
