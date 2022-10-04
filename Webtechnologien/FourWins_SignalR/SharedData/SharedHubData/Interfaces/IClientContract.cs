//-----------------------------------------------------------------------
// <copyright file="IClientContract.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SharedData.SharedHubData.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// This interface defines a client contract, which classes can choose to implement.
    /// Doing so allows those classes to be used in objects and methods requiring some form of client.
    /// </summary>
    public interface IClientContract
    {
        /// <summary>
        /// Gets the client name.
        /// </summary>
        string ClientName { get; }

        /// <summary>
        /// Send a message to the client asynchronously.
        /// </summary>
        /// <returns>A task object handling the sending of the message.</returns>
        Task SendMessageAsync(string methodName, string formattedMessage);
    }
}
