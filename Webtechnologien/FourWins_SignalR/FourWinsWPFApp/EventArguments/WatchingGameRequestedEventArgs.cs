//-----------------------------------------------------------------------
// <copyright file="WatchingGameRequestedEventArgs.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Tamara Mayer</author>
//-----------------------------------------------------------------------
using System;

namespace FourWinsWPFApp.Interfaces
{
    /// <summary>
    /// Represent a class containing information about a game that was requested to be watched.
    /// </summary>
    public class WatchingGameRequestedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WatchingGameRequestedEventArgs"/> class.
        /// </summary>
        /// <param name="GameID">The game ID of the game that is requested to be watched.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if the GameID parameter is null.
        /// </exception>
        public WatchingGameRequestedEventArgs(string GameID)
        {
            if (GameID == null)
                throw new ArgumentNullException(nameof(GameID), "GameID must not be null.");

            this.GameID = GameID;
        }

        /// <summary>
        /// Gets the game ID of the game that is requested to be watched.
        /// </summary>
        public string GameID { get; private set; }
    }
}