//-----------------------------------------------------------------------
// <copyright file="ChallengeAcceptedResponse.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SharedData.LobbyData.ResponseMessages
{
    using System;
    using SharedData.LobbyData.Interfaces;

    /// <summary>
    /// Represents a response that is sent when a challenge to a game was denied.
    /// </summary>
    public class ChallengeAcceptedResponse : IResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeDeniedResponse"/> class.
        /// </summary>
        /// <param name="challenge">The challenge that was denied.</param>
        /// <param name="message">The response message.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if challenge or message are null. 
        /// </exception>
        public ChallengeAcceptedResponse(Challenge challenge, string message)
        {
            if (challenge == null)
                throw new ArgumentNullException(nameof(challenge), "Challenge must not be null.");

            if (message == null)
                throw new ArgumentNullException(nameof(message), "Message must not be null.");

            this.Challenge = challenge;
            this.Message = message;
        }

        /// <summary>
        /// Gets the response message.
        /// </summary>
        public string Message
        {
            get;
        }

        /// <summary>
        /// Gets the challenge that was accepted.
        /// </summary>
        public Challenge Challenge
        {
            get;
        }
    }
}
