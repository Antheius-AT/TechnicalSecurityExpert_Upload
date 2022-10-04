//-----------------------------------------------------------------------
// <copyright file="ConnectionClosedEventArguments.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.EventArguments
{
    using System;

    /// <summary>
    /// Represents event arguments that contain information about why a connection was closed.
    /// </summary>
    public class ConnectionClosedEventArguments : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionClosedEventArguments"/> class.
        /// </summary>
        /// <param name="message">The information message.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if message is null.
        /// </exception>
        public ConnectionClosedEventArguments(string message)
        {
            this.InformationMessage = message ?? throw new ArgumentNullException(nameof(message), "Message must not be null.");
        }

        /// <summary>
        /// Gets the information message.
        /// </summary>
        public string InformationMessage
        {
            get;
        }
    }
}
