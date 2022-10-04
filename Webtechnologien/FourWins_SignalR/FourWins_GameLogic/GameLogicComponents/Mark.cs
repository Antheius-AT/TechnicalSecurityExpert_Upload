//-----------------------------------------------------------------------
// <copyright file="Mark.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Christian Giessrigl.</author>
//-----------------------------------------------------------------------
namespace FourWins_GameLogic.GameLogicComponents
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Represents a mark to set on a gameboard cell.
    /// </summary>
    public class Mark
    {
        /// <summary>
        /// Gets the color of the mark.
        /// </summary>
        public Color Color
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mark"/> class.
        /// </summary>
        /// <param name="color">The color of the mark.</param>
        public Mark(Color color)
        {
            if (color == null)
                throw new ArgumentNullException(nameof(color), "The color was null.");

            this.Color = color;
        }

        public override string ToString()
        {
            return this.Color.ToString();
        }
    }
}
