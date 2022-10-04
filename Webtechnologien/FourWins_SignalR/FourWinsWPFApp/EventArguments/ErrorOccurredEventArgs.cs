//-----------------------------------------------------------------------
// <copyright file="ErrorOccurredEventArgs.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.EventArguments
{
    using System;

    /// <summary>
    /// Represents event arguments that contain an error message that can be displayed in a view.
    /// </summary>
    public class ErrorOccurredEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorOccurredEventArgs"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if error message is null.
        /// </exception>
        public ErrorOccurredEventArgs(string errorMessage)
        {
            this.ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage), "Error message must not be null.");
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
