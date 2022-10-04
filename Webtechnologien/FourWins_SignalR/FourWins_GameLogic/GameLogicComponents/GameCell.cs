//-----------------------------------------------------------------------
// <copyright file="GameCell.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Christian Giessrigl, Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace FourWins_GameLogic.GameLogicComponents
{
    using System;

    /// <summary>
    /// Represents a game cell of the game board.
    /// </summary>
    public class GameCell
    {
        private Mark playerMark;

        /// <summary>
        /// Gets a value indicating whether a mark is put on the game cell or not.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return this.PlayerMark != null;
            }
        }

        /// <summary>
        /// Gets or sets the player mark of the game cell.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if you attempt to set a null reference.
        /// </exception>
        public Mark PlayerMark
        {
            get
            {
                return this.playerMark;
            }

            set
            {
                this.playerMark = value ?? throw new ArgumentNullException(nameof(value), "Value must not be null.");
            }
        }

        public override string ToString()
        {
            return $"Is Loaded:{this.IsLoaded}. By player:{this.PlayerMark}";
        }
    }
}
