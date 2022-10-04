//-----------------------------------------------------------------------
// <copyright file="Challenge.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SharedData.LobbyData
{
    using System;
    using SharedData.LobbyData.Interfaces;

    /// <summary>
    /// Represents a challenge object.
    /// </summary>
    public class Challenge : IGameChallenge<Challenge>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Challenge"/> class.
        /// </summary>
        /// <param name="challengeID">The challenge ID.</param>
        /// <param name="challengingPlayerUsername">The username of the player issuing the challenge.</param>
        /// <param name="challengedPlayerUsername">The username of the player being challenged.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either of the parameters are null.
        /// </exception>
        public Challenge(Guid challengeID, string challengingPlayerUsername, string challengedPlayerUsername, bool challengeAccepted = false)
        {
            if (challengeID == null)
                throw new ArgumentNullException(nameof(challengeID), "ID must not be null.");

            if (challengingPlayerUsername == null)
                throw new ArgumentNullException(nameof(challengingPlayerUsername), "Username of the player issuing the challenge must not be null.");

            if (challengedPlayerUsername == null)
                throw new ArgumentNullException(nameof(challengedPlayerUsername), "Username of the player being challenged must not be null.");

            this.ChallengeID = challengeID;
            this.ChallengingPlayerUsername = challengingPlayerUsername;
            this.ChallengedPlayerUsername = challengedPlayerUsername;
            this.ChallengeAccepted = challengeAccepted;
        }

        /// <summary>
        /// Gets the unique challenge ID.
        /// </summary>
        public Guid ChallengeID { get; }

        /// <summary>
        /// Gets the username of the player who was challenged.
        /// </summary>
        public string ChallengedPlayerUsername { get; }

        /// <summary>
        /// Gets the username of the player who issued the challenge.
        /// </summary>
        public string ChallengingPlayerUsername { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the challenge was accepted.
        /// </summary>
        public bool ChallengeAccepted { get; set; }

        /// <summary>
        /// Determines whether this object is equal to another instance of the same type.
        /// </summary>
        /// <param name="other">The instance of challenge to compare against.</param>
        /// <returns>Whether this instance is equal to the compared instance.</returns>
        public bool Equals(Challenge other)
        {
            return other.ChallengedPlayerUsername == this.ChallengedPlayerUsername
                && other.ChallengeID == this.ChallengeID
                && other.ChallengingPlayerUsername == this.ChallengingPlayerUsername;
        }
    }
}
