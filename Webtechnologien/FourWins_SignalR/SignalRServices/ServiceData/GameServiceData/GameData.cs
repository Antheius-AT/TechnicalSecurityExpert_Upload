//-----------------------------------------------------------------------
// <copyright file="GameData.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Christian Giessrigl.</author>
//-----------------------------------------------------------------------
namespace SignalRServices.ServiceData
{
    using FourWins_GameLogic.GameLogicComponents;
    using System;
    using System.Collections.Concurrent;

    public class GameData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameData"/> class.
        /// </summary>
        /// <param name="turnTime">The turn time.</param>
        /// <exception cref="ArgumentException">Throws if turntime is less than 1.</exception>
        public GameData(int turnTime)
        {
            if (turnTime < 1)
                throw new ArgumentException("Turntime must be greater than 0.");

            this.TurnTime = turnTime;
            this.PlayerID = Guid.NewGuid().ToString();
            this.CurrentPlayer = string.Empty;
            this.GameBoard = new GameBoard();
            this.Players = new string[2];
            this.ValidGameMovesDone = new ConcurrentQueue<QueuedGameMove>();
        }

        /// <summary>
        /// The sequence of executed game moves of this game.
        /// </summary>
        public ConcurrentQueue<QueuedGameMove> ValidGameMovesDone
        {
            get;
            internal set;
        }

        /// <summary>
        /// The ID to verify a player.
        /// </summary>
        public string PlayerID
        {
            get;
            internal set;
        }

        /// <summary>
        /// The game board.
        /// </summary>
        public GameBoard GameBoard
        {
            get;
            internal set;
        }

        /// <summary>
        /// The player names.
        /// </summary>
        public string[] Players
        {
            get;
            internal set;
        }

        /// <summary>
        /// The current players name.
        /// </summary>
        public string CurrentPlayer
        {
            get;
            set;
        }

        /// <summary>
        /// The time the current player has to make a move.
        /// </summary>
        public int TurnTime
        {
            get;
        }
    }
}
