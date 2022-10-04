//-----------------------------------------------------------------------
// <copyright file="ChallengeAcceptedEventArgs.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.EventArguments
{
    using System;
    using FourWinsWPFApp.Models;

    /// <summary>
    /// Represent event argument containing information about another player whose challenge was accepted by the client.
    /// </summary>
    public class ChallengeAcceptedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeablePlayerViewModel"/> class.
        /// </summary>
        /// <param name="playerVM">The player VM holding a reference to the player whose challenge was accepted.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if playerVM is null.
        /// </exception>
        public ChallengeAcceptedEventArgs(ChallengeablePlayerViewModel playerVM)
        {
            this.PlayerVM = playerVM ?? throw new ArgumentNullException(nameof(playerVM), "Player view model must not be null.");
        }

        /// <summary>
        /// Gets the playerVM holding a reference to the player whose challenge was accepted.
        /// </summary>
        public ChallengeablePlayerViewModel PlayerVM
        {
            get;
        }
    }
}
