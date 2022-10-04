//-----------------------------------------------------------------------
// <copyright file="UpdateClientListQueueItem.cs" company="FHWN">
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
    /// This class represent a todo object, which transmits the client list to the specified client contained in
    /// the abstract client contract type.
    /// </summary>
    public class UpdateClientListQueueItem : IQueueItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateClientListQueueItem"/> class.
        /// </summary>
        /// <param name="receiver">The client to forward the data to.</param>
        /// <param name="methodName">The method to invoke on the client.</param>
        /// <param name="response">The respone for the client.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either of the parameters are null.
        /// </exception>
        public UpdateClientListQueueItem(IClientContract receiver, string methodName, InitializePlayerListResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response), "Formatted response data must not be null.");

            if (receiver == null)
                throw new ArgumentNullException(nameof(receiver), "forward to must not be null.");

            if (methodName == null)
                throw new ArgumentNullException(nameof(methodName), "method name must not be null.");

            this.Response = response;
            this.Receiver = receiver;
            this.MethodName = methodName;
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
