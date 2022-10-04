//-----------------------------------------------------------------------
// <copyright file="NewGameCreatedResponse.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SharedData.LobbyData.ResponseMessages
{
    using System;
    using SharedData.LobbyData.Interfaces;

    /// <summary>
    /// Represents a response containing information about a newly created game.
    /// </summary>
    public class NewGameCreatedResponse : IResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NewGameCreatedResponse"/> class.
        /// </summary>
        /// <param name="message">The response message.</param>
        /// <param name="gameID">The ID of the created game.</param>
        /// <param name="playerID">The ID for players of the game.</param>
        /// <param name="playerOneUsername">The username of player one.</param>
        /// <param name="playerTwoUsername">The username of player two.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either of the parameters are null.
        /// </exception>
        public NewGameCreatedResponse(string message, string gameID, string playerID, string playerOneUsername, string playerTwoUsername)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message), "Message must not be null.");

            if (gameID == null)
                throw new ArgumentNullException(nameof(gameID), "Game ID must not be null.");

            if (playerID == null)
                throw new ArgumentNullException(nameof(playerID), "Player ID must not be null.");

            if (playerOneUsername == null)
                throw new ArgumentNullException(nameof(playerOneUsername), "Player one username must not be null.");

            if (playerTwoUsername == null)
                throw new ArgumentNullException(nameof(playerTwoUsername), "Player one username must not be null.");

            this.Message = message;
            this.PlayerID = playerID;
            this.GameID = gameID;
            this.PlayerOneUsername = playerOneUsername;
            this.PlayerTwoUsername = playerTwoUsername;
        }

        /// <summary>
        /// Gets the player ID.
        /// </summary>
        public string PlayerID
        {
            get;
        }

        /// <summary>
        /// Gets the game ID.
        /// </summary>
        public string GameID
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

        /// <summary>
        /// Gets the username of player one.
        /// </summary>
        public string PlayerOneUsername
        {
            get;
        }

        /// <summary>
        /// Gets the username of player two.
        /// </summary>
        public string PlayerTwoUsername
        {
            get;
        }
    }
}
