//-----------------------------------------------------------------------
// <copyright file="GameRemovedResponse.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SharedData.LobbyData.ResponseMessages
{
    using System;
    using SharedData.LobbyData.Interfaces;

    /// <summary>
    /// This class represent a response that informs a client that a game is no longer active 
    /// and should not be listed as available anymore.
    /// </summary>
    public class GameRemovedResponse : IResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameRemovedResponse"/> class.
        /// </summary>
        /// <param name="message">The response message.</param>
        /// <param name="removedGameID">The game ID of the removed game.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either of the parameters are null.
        /// </exception>
        public GameRemovedResponse(string message, string removedGameID)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message), "Message must not be null.2");

            if (removedGameID == null)
                throw new ArgumentNullException(nameof(removedGameID), "Game ID must not be null.");

            this.Message = message;
            this.RemovedGameID = removedGameID;
        }

        /// <summary>
        /// Gets the response message.
        /// </summary>
        public string Message
        {
            get;
        }

        /// <summary>
        /// Gets the game ID of the removed game.
        /// </summary>
        public string RemovedGameID
        {
            get;
        }
    }
}
