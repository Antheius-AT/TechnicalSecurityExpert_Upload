//-----------------------------------------------------------------------
// <copyright file="ConnectionInfo.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.Models
{
    using System;
    using System.Net;

    /// <summary>
    /// Represents a class containing information required for logging.
    /// </summary>
    public class ConnectionInfo
    {
        private string username;
        private Uri serverURL;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionInfo"/> class using default values for username and server URL.
        /// </summary>
        public ConnectionInfo()
        {
            this.Username = "Default";
            this.ServerURL = new Uri($"http://{IPAddress.Loopback}:80");
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if username is null.
        /// </exception>
        public string Username
        {
            get
            {
                return this.username;
            }

            set
            {
                this.username = value ?? throw new ArgumentNullException(nameof(value), "Value must not be null.");
            }
        }

        /// <summary>
        /// Gets or sets the server URL.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if server URL is null.
        /// </exception>
        public Uri ServerURL
        {
            get
            {
                return this.serverURL;
            }

            set
            {
                this.serverURL = value ?? throw new ArgumentNullException(nameof(value), "Server URL must not be null.");
            }
        }
    }
}
