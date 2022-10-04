//-----------------------------------------------------------------------
// <copyright file="ChallengeRequest.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SharedData.LobbyData.Requests
{
    using System;
    using SharedData.LobbyData.Interfaces;

    /// <summary>
    /// Represents a request to the server to create a challenge object.
    /// </summary>
    public class ChallengeRequest : IRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChallengeRequest"/> class.
        /// </summary>
        /// <param name="challengeIssuer">The challenge issuer.</param>
        /// <param name="challengeReceiver">The challenge receiver.</param>
        /// <param name="message">The message for this request.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either of the parameters are null.
        /// </exception>
        public ChallengeRequest(string challengeIssuer, string challengeReceiver, string message)
        {
            this.ChallengeIssuer = challengeIssuer;
            this.ChallengeReceiver = challengeReceiver;
            this.Message = message;
        }

        /// <summary>
        /// Gets the challenge receiver.
        /// </summary>
        public string ChallengeReceiver
        {
            get;
        }

        /// <summary>
        /// Gets the challenge issuer.
        /// </summary>
        public string ChallengeIssuer
        {
            get;
        }
        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message
        {
            get;
        }
    }
}
