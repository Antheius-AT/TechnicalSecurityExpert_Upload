//-----------------------------------------------------------------------
// <copyright file="SignalRCompositeClient.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace FourWins_SignalRServer.ClientContract
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using SharedData.SharedHubData.Interfaces;

    /// <summary>
    /// This class represents a composite collection of signal R clients that can be used 
    /// for sending a message to multiple clients at once, instead of just a single one.
    /// </summary>
    public class SignalRCompositeClient : IClientContract
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignalRClient"/> class.
        /// </summary>
        /// <param name="username">The client username.</param>
        /// <param name="clientProxy">The client proxy through which to send messages.</param>
        /// <param name="connectionID">The clients connection ID.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if client proxy is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Is thrown if username is null, empty or only consisting of white space characters.
        /// </exception>
        public SignalRCompositeClient(IEnumerable<ISignalRClient> clients)
        {
            if (clients == null)
                throw new ArgumentNullException(nameof(clients), "clients must not be null.");

            this.Clients = clients;
        }

        /// <summary>
        /// Gets the enumerable collection of clients.
        /// </summary>
        public IEnumerable<ISignalRClient> Clients
        {
            get;
        }

        /// <summary>
        /// Gets the client name.
        /// </summary>
        public string ClientName
        {
            get
            {
                return "CompositeClient";
            }
        }

        /// <summary>
        /// Send a message to each of the client contained in this composite client.
        /// </summary>
        /// <param name="methodName">The method name to invoke on the clients.</param>
        /// <param name="formattedMessage">The formatted message containing the data.</param>
        /// <returns>A task object handling the sending of the message.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if method name or formatted message are null.
        /// </exception>
        public Task SendMessageAsync(string methodName, string formattedMessage)
        {
            if (methodName == null)
                throw new ArgumentNullException(nameof(methodName), "Method name must not be null.");

            if (formattedMessage == null)
                throw new ArgumentNullException(nameof(formattedMessage), "Formatted message must not be null.");

            return Task.Run(async () =>
            {
                foreach (var item in this.Clients)
                {
                    await item.ClientProxy.SendAsync(methodName, formattedMessage);
                }
            });
        }
    }
}
