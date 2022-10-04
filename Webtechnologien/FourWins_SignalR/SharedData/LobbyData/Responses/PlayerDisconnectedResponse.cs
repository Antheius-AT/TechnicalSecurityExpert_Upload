//-----------------------------------------------------------------------
// <copyright file="PlayerDisconnectedResponse.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SharedData.LobbyData.ResponseMessages
{
    using System;
    using SharedData.LobbyData.Interfaces;

    /// <summary>
    /// Represents a response that is issued when a player connected.
    /// </summary>
    public class PlayerDisconnectedResponse : IResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerConnectedResponse"/> class.
        /// </summary>
        /// <param name="message">The response message.</param>
        /// <param name="disconnectedPlayerUsername">The disconnected player username.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if either of the parameters are null.
        /// </exception>
        public PlayerDisconnectedResponse(string message, string disconnectedPlayerUsername)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message), "Message must not be null");

            if (disconnectedPlayerUsername == null)
                throw new ArgumentNullException(nameof(disconnectedPlayerUsername), "Player username must not be null.");

            this.Message = message;
            this.DisconnectedPlayerUsername = disconnectedPlayerUsername;
        }

        /// <summary>
        /// Gets the username of the disconnected player.
        /// </summary>
        public string DisconnectedPlayerUsername
        {
            get;
        }

        /// <summary>
        /// Gets the response message.
        /// </summary>
        public string Message
        {
            get;
        }
    }
}
