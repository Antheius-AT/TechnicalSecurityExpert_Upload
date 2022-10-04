//-----------------------------------------------------------------------
// <copyright file="WatchableGame.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
namespace SharedData.LobbyData
{
    using System;

    /// <summary>
    /// This class represents a game that is currently in progress and which the client can watch.
    /// </summary>
    public class WatchableGame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WatchableGame"/> class.
        /// </summary>
        /// <param name="gameID">The game ID.</param>
        /// <param name="playerOne">The first player playing in this game.</param>
        /// <param name="playerTwo">The second player playing in this game.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either of the parameters are null.
        /// </exception>
        public WatchableGame(string gameID, string playerOne, string playerTwo)
        {
            if (gameID == null)
                throw new ArgumentNullException(nameof(gameID), "Game ID must not be null.");

            if (playerOne == null)
                throw new ArgumentNullException(nameof(playerOne), "Player one must not be null.");

            if (playerTwo == null)
                throw new ArgumentNullException(nameof(playerTwo), "Player two must not be null.");

            this.GameID = gameID;
            this.PlayerOne = playerOne;
            this.PlayerTwo = playerTwo;
        }

        /// <summary>
        /// Gets the game ID that is needed when connecting to the game hub as a spectator.
        /// </summary>
        public string GameID
        {
            get;
        }

        /// <summary>
        /// Gets player one of this game.
        /// </summary>
        public string PlayerOne
        {
            get;
        }

        /// <summary>
        /// Gets player two of this game.
        /// </summary>
        public string PlayerTwo
        {
            get;
        }
    }
}
