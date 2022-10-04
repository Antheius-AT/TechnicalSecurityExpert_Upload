//-----------------------------------------------------------------------
// <copyright file="IRequestMessage.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SharedData.LobbyData.Interfaces
{
    /// <summary>
    /// Represents a request message sent from the client to the server.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Gets the message that sent with this request object.
        /// </summary>
        string Message { get; }
    }
}
