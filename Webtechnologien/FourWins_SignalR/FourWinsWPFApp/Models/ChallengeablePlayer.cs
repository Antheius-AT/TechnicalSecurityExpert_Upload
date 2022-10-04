//-----------------------------------------------------------------------
// <copyright file="ChallengeablePlayer.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.Models
{
    using System;

    /// <summary>
    /// This class represents a visualization of other players that the client can challenge.
    /// </summary>
    /// TODO: Make this a view model.
    public class ChallengeablePlayer
    {
        private ChallengeStatus challengeStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeablePlayer"/> class.
        /// </summary>
        /// <param name="username">The players username.</param>
        public ChallengeablePlayer(string username)
        {
            this.Username = username ?? throw new ArgumentNullException(nameof(username), "Username must not be null.");
            this.ChallengeStatus = ChallengeStatus.Available;
        }

        /// <summary>
        /// Gets the players username.
        /// </summary>
        public string Username
        {
            get;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this player is challenging the client.
        /// </summary>
        public ChallengeStatus ChallengeStatus
        {
            get
            {
                return this.challengeStatus;
            }

            set
            {
                this.challengeStatus = value;
            }
        }
    }
}
