//-----------------------------------------------------------------------
// <copyright file="FormatconversionException.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SharedData.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an exception that is thrown if format conversion fails.
    /// </summary>
    public class FormatConversionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatConversionException"/> class
        /// </summary>
        public FormatConversionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatConversionException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public FormatConversionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatConversionException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public FormatConversionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatConversionException"/> class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        public FormatConversionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
