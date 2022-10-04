//-----------------------------------------------------------------------
// <copyright file="InvalidUsernameEventArguments.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.EventArguments
{
    using System;

    /// <summary>
    /// Represent event arguments that contain information about why a username was invalid.
    /// </summary>
    public class InvalidUsernameEventArguments : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidUsernameEventArguments"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>+
        /// <exception cref="ArgumentNullException">
        /// Thrown if message is null.
        /// </exception>
        public InvalidUsernameEventArguments(string message)
        {
            this.ErrorMessage = message ?? throw new ArgumentNullException(nameof(message), "Message must not be null.");
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string ErrorMessage
        {
            get;
        }
    }
}
