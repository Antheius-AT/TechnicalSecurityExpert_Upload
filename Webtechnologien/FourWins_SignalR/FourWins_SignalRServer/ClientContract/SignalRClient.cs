//-----------------------------------------------------------------------
// <copyright file="SignalRClient.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace FourWins_SignalRServer.ClientContract
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;

    /// <summary>
    /// Represent a signalR client.
    /// </summary>
    public class SignalRClient : ISignalRClient
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
        public SignalRClient(string username, IClientProxy clientProxy, string connectionID)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException(nameof(username), "Username must not be null, empty or only consisting of white space characters.");

            if (clientProxy == null)
                throw new ArgumentNullException(nameof(clientProxy), "Client proxy must not be null.");

            if(connectionID == null)
                throw new ArgumentNullException(nameof(connectionID), "Connection id must not be null.");

            this.ClientName = username;
            this.ClientProxy = clientProxy;
            this.ConnectionID = connectionID;
        }

        /// <summary>
        /// Gets the clients username.
        /// </summary>
        public string ClientName
        {
            get;
        }

        /// <summary>
        /// Gets the client proxy through which to send messages.
        /// </summary>
        public IClientProxy ClientProxy
        {
            get;
        }

        /// <summary>
        /// Gets the connection id of the client.
        /// </summary>
        public string ConnectionID 
        {
            get;
        }

        /// <summary>
        /// Compares this instance against another instance of the same type and returns
        /// a value indicating whether they are the same.
        /// </summary>
        /// <param name="other">The other client object to compare.</param>
        /// <returns>0 if the objects are equal. -1 if they are not equal.</returns>
        public int CompareTo([AllowNull] ISignalRClient other)
        {
            if (this.ClientName == other.ClientName && this.ClientProxy == other.ClientProxy && this.ConnectionID == other.ConnectionID)
                return 0;

            return -1;
        }

        /// <summary>
        /// Sends a message to the specified client proxy.
        /// </summary>
        /// <param name="methodName">The method name to send to the client proxy.</param>
        /// <param name="formattedMessage">The formatted message data.</param>
        /// <returns>A task object handling the sending of the message.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if method name or formatted message are null.
        /// </exception>
        public async Task SendMessageAsync(string methodName, string formattedMessage)
        {
            if (methodName == null)
                throw new ArgumentNullException(nameof(methodName), "Method name must not be null.");

            if (formattedMessage == null)
                throw new ArgumentNullException(nameof(formattedMessage), "Formatted message must not be null.");

            await this.ClientProxy.SendAsync(methodName, formattedMessage);
        }
    }
}
