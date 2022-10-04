//-----------------------------------------------------------------------
// <copyright file="GameStateData.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Christian Giessrigl.</author>
//-----------------------------------------------------------------------
namespace SharedData.GameData
{
    using Newtonsoft.Json;
    using SignalRServices.ServiceData;
    using System.Collections.Concurrent;

    public class GameStateData
    {
        /// <summary>
        /// The sequence of executed game moves of this game.
        /// </summary>
        [JsonProperty("Moves")]
        public ConcurrentQueue<QueuedGameMove> ValidGameMovesDone
        {
            get;
            set;
        }

        /// <summary>
        /// The player names.
        /// </summary>
        [JsonProperty("Players")]
        public string[] Players
        {
            get;
            set;
        }

        /// <summary>
        /// The current players name.
        /// </summary>
        [JsonProperty("CurrentPlayer")]
        public string CurrentPlayer
        {
            get;
            set;
        }

        /// <summary>
        /// The ID of the game.
        /// </summary>
        [JsonProperty("GameID")]
        public string GameID
        {
            get;
            set;
        }
    }
}
