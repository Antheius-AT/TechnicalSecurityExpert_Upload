//-----------------------------------------------------------------------
// <copyright file="IGameChallenge.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SharedData.LobbyData.Interfaces
{
    using System;

    /// <summary>
    /// Defines an object that represent a challenge that is currently active and waiting for a response.
    /// </summary>
    public interface IGameChallenge<TEquatable>: IEquatable<TEquatable>
    {
        /// <summary>
        /// Gets the unique challenge ID.
        /// </summary>
        Guid ChallengeID { get; }

        /// <summary>
        /// Gets the username of the challenged player.
        /// </summary>
        string ChallengedPlayerUsername { get; }

        /// <summary>
        /// Gets the username of the player who has issued the challenge.
        /// </summary>
        string ChallengingPlayerUsername { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this challenge has already been accepted.
        /// </summary>
        bool ChallengeAccepted { get; set; }
    }
}
