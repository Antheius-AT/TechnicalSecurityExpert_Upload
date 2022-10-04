//-----------------------------------------------------------------------
// <copyright file="ColumnFullyLoadedException.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Christian Giessrigl</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.GameLogic.Exceptions
{
    using System;

    /// <summary>
    /// Represents an exception if the game board column is fully loaded.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ColumnFullyLoadedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnFullyLoadedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ColumnFullyLoadedException(string message) : base(message)
        {
        }
    }
}
