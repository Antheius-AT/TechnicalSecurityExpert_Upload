//-----------------------------------------------------------------------
// <copyright file="IQueueItemVisitable.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SignalRServices.Interfaces.DataInterfaces
{
    using SharedData.LobbyData.Interfaces;
    using SharedData.SharedHubData.Interfaces;

    /// <summary>
    /// Represent a queue item visitable.
    /// </summary>
    public interface IQueueItem
    {
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
