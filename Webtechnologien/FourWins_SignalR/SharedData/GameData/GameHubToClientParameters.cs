//-----------------------------------------------------------------------
// <copyright file="GameHubToClientParameters.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Christian Giessrigl.</author>
//-----------------------------------------------------------------------
namespace FourWins_SignalRServer.HubData
{
    using Newtonsoft.Json;

    public class GameHubToClientParameters
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
        /// The name of the player.
        /// </summary>
        [JsonProperty("PlayerName")]
        public string PlayerName
        { 
            get;
            set;
        }

        /// <summary>
        /// The column of the game board from the last move if one was made.
        /// </summary>
        [JsonProperty("Column")]
        public int Column
        { 
            get;
            set;
        }

        /// <summary>
        /// A value indicating if the client is a player of the game.
        /// </summary>
        [JsonProperty("IsPlayer")]
        public bool IsPlayer
        { 
            get;
            set;
        }

        /// <summary>
        /// A value indicating if it is the clients turn in the game.
        /// </summary>
        [JsonProperty("IsCurrentPlayer")]
        public bool IsCurrentPlayer
        {
            get;
            set;
        }
    }
}
