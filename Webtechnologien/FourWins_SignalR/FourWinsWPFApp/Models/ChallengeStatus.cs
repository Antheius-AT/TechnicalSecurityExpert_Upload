//-----------------------------------------------------------------------
// <copyright file="ChallengeStatus.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.Models
{
    /// <summary>
    /// Represent an enumeration holding challenge statuses.
    /// </summary>
    public enum ChallengeStatus
    {
        /// <summary>
        /// Represent an incoming challenge.
        /// </summary>
        ChallengeIncoming,

        /// <summary>
        /// Represents an outgoing challenge.
        /// </summary>
        ChallengeOutgoing,

        /// <summary>
        /// Represents a client that is available for a challenge.
        /// </summary>
        Available,

        /// <summary>
        /// Represents a client wich which a game is currently being created on the server.
        /// </summary>
        CreatingMatch
    }
}
