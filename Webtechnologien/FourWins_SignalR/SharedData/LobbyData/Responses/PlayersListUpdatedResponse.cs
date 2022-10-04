//-----------------------------------------------------------------------
// <copyright file="PlayersListUpdatedResponse.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SharedData.LobbyData.ResponseMessages
{
    using System;
    using System.Collections.Generic;
    using SharedData.LobbyData.Interfaces;

    /// <summary>
    /// Represents a response that is issued when the player list was updated.
    /// </summary>
    public class PlayersListUpdatedResponse : IResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayersListUpdatedResponse"/> class.
        /// </summary>
        /// <param name="message">The response message.</param>
        /// <param name="playerUsernames">The usernames of connected players.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if message or player usernames are null.
        /// </exception>
        public PlayersListUpdatedResponse(string message, IEnumerable<string> playerUsernames)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message), "Message must not be null.");

            if (playerUsernames == null)
                throw new ArgumentNullException(nameof(playerUsernames), "Usernames must not be null.");

            this.Message = message;
            this.PlayerUsernames = playerUsernames;
        }

        /// <summary>
        /// Gets the enumerable collection of connected player usernames.
        /// </summary>
        public IEnumerable<string> PlayerUsernames
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
