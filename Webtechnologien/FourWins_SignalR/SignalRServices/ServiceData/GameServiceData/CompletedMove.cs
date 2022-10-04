//-----------------------------------------------------------------------
// <copyright file="CompletedMove.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman, Christian Giessrigl.</author>
//-----------------------------------------------------------------------
using System;
using FourWins_GameLogic.GameLogicComponents;

namespace SignalRServices.ServiceData.GameServiceData
{
    /// <summary>
    /// This class represents a previously completed move.
    /// </summary>
    public class CompletedMove
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompletedMove"/> class.
        /// </summary>
        /// <param name="playerMark">The placed mark.</param>
        /// <param name="column">The column in which the mark was placed.</param>
        /// <param name="row">The row in which the mark was placed.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if player mark is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Is thrown if column or row are negative.
        /// </exception>
        public CompletedMove(Mark playerMark, int column, int row)
        {
            if (playerMark == null)
                throw new ArgumentNullException(nameof(playerMark), "Player mark must not be null.");

            if (column < 0)
                throw new ArgumentOutOfRangeException(nameof(column), "Column must not be negative.");

            if (row < 0)
                throw new ArgumentOutOfRangeException(nameof(row), "Row must not be negative.");

            this.PlayerMark = playerMark;
            this.Row = row;
            this.Column = column;
        }

        /// <summary>
        /// Gets the player mark which was placed.
        /// </summary>
        public Mark PlayerMark
        {
            get;
        }

        /// <summary>
        /// Gets the row this move was placed in.
        /// </summary>
        public int Row
        {
            get;
        }

        /// <summary>
        /// Gets the column this move was placed in.
        /// </summary>
        public int Column
        {
            get;
        }
    }
}
