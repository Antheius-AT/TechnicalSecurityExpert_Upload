using System;
//-----------------------------------------------------------------------
// <copyright file="GameCreationRequestedEventArgs.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.EventArguments
{
    /// <summary>
    /// Represents event arguments containing information about a game which was started
    /// on the server.
    /// </summary>
    public class GameCreationRequestedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameCreationRequestedEventArgs"/> class.
        /// </summary>
        /// <param name="playerID">The player ID.</param>
        /// <param name="gameID">The game ID.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either of the parameters are null.
        /// </exception>
        public GameCreationRequestedEventArgs(string playerID, string gameID, string playerOne, string playerTwo)
        {
            if (playerID == null)
                throw new ArgumentNullException(nameof(playerID), "Player ID must not be null.");

            if (gameID == null)
                throw new ArgumentNullException(nameof(gameID), "Game ID must not be null.");

            if (playerOne == null)
                throw new ArgumentNullException(nameof(gameID), "Game ID must not be null.");

            if (playerTwo == null)
                throw new ArgumentNullException(nameof(gameID), "Game ID must not be null.");

            this.PlayerID = playerID;
            this.GameID = gameID;
            this.PlayerOne = playerOne;
            this.PlayerTwo = playerTwo;
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
        /// The name of player one.
        /// </summary>
        public string PlayerOne
        {
            get;
        }

        /// <summary>
        /// The name of player two
        /// </summary>
        public string PlayerTwo
        {
            get;
        }
    }
}
