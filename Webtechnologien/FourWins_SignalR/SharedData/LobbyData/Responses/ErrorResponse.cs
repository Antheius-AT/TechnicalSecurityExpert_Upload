//-----------------------------------------------------------------------
// <copyright file="ErrorMessage.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
using System;
using SharedData.LobbyData.Interfaces;

namespace SharedData.LobbyData.ResponseMessages
{
    /// <summary>
    /// Represents an error response informing the client that something went wrong.
    /// </summary>
    public class ErrorResponse : IResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResponse"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ErrorResponse(string message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message), "Message must not be null.");

            this.Message = message;
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string Message
        {
            get;
        }
    }
}
