//-----------------------------------------------------------------------
// <copyright file="IssuedChallenge.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
using System;
using SharedData.LobbyData.Interfaces;

namespace SharedData.LobbyData.ResponseMessages
{
    /// <summary>
    /// This class represents a challenge that was issued and forwarded to the challenged player.
    /// </summary>
    public class IssuedChallengeResponse : IResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IssuedChallengeResponse"/> class.
        /// </summary>
        /// <param name="challengeID">The challenge ID.</param>
        /// <param name="challengingPlayerName">The name of the player who is issuing the challenge.</param>
        /// <param name="challengedPlayerName">The name of the player who is being challenged.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either of the parameters are null.
        /// </exception>
        public IssuedChallengeResponse(Challenge challenge, string message)
        {
            if (challenge == null)
                throw new ArgumentNullException(nameof(challenge), "Issued challenge must not be null.");

            if (message == null)
                throw new ArgumentNullException(nameof(message), "Message must not be null.");

            this.Message = message;
            this.Challenge = challenge;
        }

        /// <summary>
        /// Gets the issued challenge.
        /// </summary>
        public Challenge Challenge
        {
            get;
        }

        /// <summary>
        /// Gets the issued challenge message.
        /// </summary>
        public string Message
        {
            get;
        }
    }
}
