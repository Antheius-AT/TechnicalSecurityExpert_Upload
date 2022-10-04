//-----------------------------------------------------------------------
// <copyright file="SignalRGameClient.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Christian Giessrigl.</author>
//-----------------------------------------------------------------------
namespace FourWins_SignalRServer.ClientContract
{
    using Microsoft.AspNetCore.SignalR;
    using System.Collections.Concurrent;

    public class SignalRGameClient : SignalRClient
    {
        /// <summary>
        /// The games the user is involved in.
        /// </summary>
        public  readonly ConcurrentDictionary<string, string> games;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalRGameClient"/> class.
        /// </summary>
        /// <param name="username">The name of the user.</param>
        /// <param name="clientProxy">The client proxy through which to send messages.</param>
        /// <param name="connectionID">The connection id of the client.</param>
        public SignalRGameClient(string username, IClientProxy clientProxy, string connectionID) : base(username, clientProxy, connectionID)
        {
            this.games = new ConcurrentDictionary<string, string>();
        }
    }
}
