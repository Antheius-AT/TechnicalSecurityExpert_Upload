//-----------------------------------------------------------------------
// <copyright file="PlayerConnectedResponse.cs" company="FHWN">
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
    public class PlayerConnectedResponse : IResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerConnectedResponse"/> class.
        /// </summary>
        /// <param name="message">The response message.</param>
        /// <param name="playerUsername">The connected player username.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if either of the parameters are null.
        /// </exception>
        public PlayerConnectedResponse(string message, string playerUsername)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message), "Message must not be null");

            if (playerUsername == null)
                throw new ArgumentNullException(nameof(playerUsername), "Player username must not be null.");

            this.Message = message;
            this.PlayerUsername = playerUsername;
        }

        /// <summary>
        /// Gets the username of the connected player.
        /// </summary>
        public string PlayerUsername
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
