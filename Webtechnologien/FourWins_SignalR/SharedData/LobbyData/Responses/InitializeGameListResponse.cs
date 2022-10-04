//-----------------------------------------------------------------------
// <copyright file="InitializeGameListResponse.cs" company="FHWN">
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
    public class InitializeGameListResponse : IResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializePlayerListResponse"/> class.
        /// </summary>
        /// <param name="message">The response message.</param>
        /// <param name="games">The currently active games.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if message or player usernames are null.
        /// </exception>
        public InitializeGameListResponse(string message, IEnumerable<WatchableGame> games)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message), "Message must not be null.");

            if (games == null)
                throw new ArgumentNullException(nameof(games), "Usernames must not be null.");

            this.Message = message;
            this.Games = games;
        }

        /// <summary>
        /// Gets the enumerable collection of connected player usernames.
        /// </summary>
        public IEnumerable<WatchableGame> Games
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
