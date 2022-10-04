//-----------------------------------------------------------------------
// <copyright file="QueuedGameMove.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Christian Giessrigl.</author>
//-----------------------------------------------------------------------
namespace SignalRServices.ServiceData
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a single game move that is issued into a queue for later handling.
    /// </summary>
    public class QueuedGameMove
    {
        /// <summary>
        /// Gets the name of the player who made the move.
        /// </summary>
        [JsonProperty("PlayerName")]
        public string PlayerName
        { 
            get; 
        }

        /// <summary>
        /// Gets the row of the gameboard in which the move has been made.
        /// </summary>
        [JsonProperty("Column")]
        public int Column
        {
            get;
        }

        /// <summary>
        /// Initializes a new Instance of the <see cref="QueuedGameMove"/> class.
        /// </summary>
        /// <param name="playerName">The name of the player who made the move.</param>
        /// <param name="column">The row of the gameboard in which the move has been made.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if player name is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Is thrown if column is negative.
        /// </exception>
        public QueuedGameMove(string playerName, int column)
        {
            if (playerName == null)
                throw new ArgumentNullException(nameof(playerName), "Player name must not be null.");

            if (column < 0)
                throw new ArgumentOutOfRangeException(nameof(column), "Column must not be negative.");

            this.PlayerName = playerName;
            this.Column = column;
        }
    }
}
