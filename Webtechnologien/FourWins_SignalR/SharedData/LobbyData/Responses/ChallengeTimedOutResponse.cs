//-----------------------------------------------------------------------
// <copyright file="ChallengeTimedOutResponse.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
using System;
using SharedData.LobbyData.Interfaces;

namespace SharedData.LobbyData.ResponseMessages
{
    /// <summary>
    /// Represent a response that is sent when a challenge timed out.
    /// </summary>
    public class ChallengeTimedOutResponse : IResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeTimedOutResponse"/> class.
        /// </summary>
        /// <param name="challenge">The challenge that timed out.</param>
        /// <param name="message">The response message.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if challenge or message are null.
        /// </exception>
        public ChallengeTimedOutResponse(Challenge challenge, string message)
        {
            if (challenge == null)
                throw new ArgumentNullException(nameof(challenge), "Challenge must not be null.");

            if (message == null)
                throw new ArgumentNullException(nameof(message), "Message must not be null.");

            this.Challenge = challenge;
            this.Message = message;
        }

        /// <summary>
        /// Gets the challenge that timed out.
        /// </summary>
        public Challenge Challenge
        {
            get;
        }

        /// <summary>
        /// Gets the response message.
        /// </summary>
        public string Message
        {
            get;
        }
    }
}
