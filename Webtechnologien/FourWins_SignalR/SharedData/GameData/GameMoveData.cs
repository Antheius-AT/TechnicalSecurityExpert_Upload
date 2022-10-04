//-----------------------------------------------------------------------
// <copyright file="GameMoveData.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Christian Giessrigl.</author>
//-----------------------------------------------------------------------
namespace SignalRServices.ServiceData
{
    using Newtonsoft.Json;

    public class GameMoveData
    {
        /// <summary>
        /// The ID of the game.
        /// </summary>
        [JsonProperty("GameID")]
        public string GameID
        { 
            get; 
        }

        /// <summary>
        /// True if the move is valid. False if not.
        /// </summary>
        [JsonProperty("IsValid")]
        public bool IsValid
        {
            get;
        }

        /// <summary>
        /// True if the gameboard is full. False if not.
        /// </summary>
        [JsonProperty("IsGameBoardFull")]
        public bool IsGameBoardFull
        {
            get;
        }

        /// <summary>
        /// True if the player who made the move wins the game. False if not.
        /// </summary>
        [JsonProperty("HasWon")]
        public bool HasWon
        { 
            get;
        }

        /// <summary>
        /// The name of the current player.
        /// </summary>
        [JsonProperty("CurrentPlayer")]
        public string CurrentPlayer
        {
            get;
        }

        /// <summary>
        /// The column of the gameboard in which the player made a move.
        /// </summary>
        [JsonProperty("Column")]
        public int Column
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameMoveData"/> class.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <param name="currentPlayer">The name of the player.</param>
        /// <param name="isValid">A value indicating if the move is valid.</param>
        /// <param name="isGameBoardFull">A value indicating if the gameboard is full.</param>
        /// <param name="hasWon">A value indicating if the player wins after the move.</param>
        /// <param name="column">The column of the gameboard the player made a move in.</param>
        public GameMoveData(string gameID, string currentPlayer, bool isValid, bool isGameBoardFull, bool hasWon, int column)
        {
            this.GameID = gameID;
            this.CurrentPlayer = currentPlayer;
            this.IsGameBoardFull = isGameBoardFull;
            this.IsValid = isValid;
            this.HasWon = hasWon;
            this.Column = column;
        }
    }
}
