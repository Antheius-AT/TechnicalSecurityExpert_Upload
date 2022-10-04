//-----------------------------------------------------------------------
// <copyright file="ClientToGameHubParameters.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Christian Giessrigl.</author>
//-----------------------------------------------------------------------
namespace FourWins_SignalRServer.HubData
{
    using Newtonsoft.Json;

    public class ClientToGameHubParameters
    {
        /// <summary>
        /// The ID of the game.
        /// </summary>
        [JsonProperty("GameID")]
        public string GameID
        {
            get;
            set;
        }

        /// <summary>
        /// The ID to verify as a player of the game.
        /// </summary>
        [JsonProperty("PlayerID")]
        public string PlayerID
        {
            get;
            set;
        }

        /// <summary>
        /// The username of the client.
        /// </summary>
        [JsonProperty("PlayerName")]
        public string PlayerName
        {
            get;
            set;
        }

        /// <summary>
        /// The column of the gameboard when the client wants to make a move.
        /// </summary>
        [JsonProperty("Column")]
        public int Column
        {
            get;
            set;
        }
    }
}
