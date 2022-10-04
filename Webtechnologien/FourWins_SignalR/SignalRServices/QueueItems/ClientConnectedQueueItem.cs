//-----------------------------------------------------------------------
// <copyright file="ClientConnectedQueueItem.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SignalRServices.QueueItems
{
    using System;
    using SharedData.LobbyData.Interfaces;
    using SharedData.LobbyData.ResponseMessages;
    using SharedData.SharedHubData.Interfaces;
    using SignalRServices.Interfaces.DataInterfaces;

    /// <summary>
    /// This class represent a queue item signaling that a new client connected.
    /// </summary>
    public class ClientConnectedQueueItem : IQueueItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConnectedQueueItem"/> class.
        /// </summary>
        /// <param name="receiver">The client to forward the knowledge to, that a new client connected.</param>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="response">The response for the client.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either of the parameters are null..
        /// </exception>
        public ClientConnectedQueueItem(IClientContract receiver, string methodName, PlayerConnectedResponse response)
        {
            if (receiver == null)
                throw new ArgumentNullException(nameof(receiver), "Client to forward to must not be null.");

            if (methodName == null)
                throw new ArgumentNullException(nameof(methodName), "Method name must not be null.");

            this.Receiver = receiver;
            this.MethodName = methodName;
            this.Response = response;
        }

        /// <summary>
        /// Gets the receiver of this queue item.
        /// </summary>
        public IClientContract Receiver { get; }

        /// <summary>
        /// Gets the method name of the receiver this queue item invokes.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Gets the response contained within this queue item.
        /// </summary>
        public IResponse Response { get; }
    }
}
