//-----------------------------------------------------------------------
// <copyright file="Game.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Tamara Mayer</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.Models
{
    using FourWins_GameLogic.Commands;
    using FourWins_GameLogic.GameLogicComponents;
    using System;

    public class Game
    {
        /// <summary>
        /// Is a instance of the <see cref="GameBoard"/>.
        /// </summary>
        public GameBoard GameBoard { get; private set; }

        /// <summary>
        /// Is the username of ther player whose turn it is.
        /// </summary>
        public string TurnOf_UserName
        {
            get
            {
                return this.turnOf_UserName;
            }
            set
            {
                if (value == this.PlayerOne || value == this.PlayerTwo)
                {
                    this.turnOf_UserName = value;
                }
            }
        }

        /// <summary>
        /// The <see cref="Mark"/> for player one.
        /// </summary>
        private readonly Mark playerOneMark;

        /// <summary>
        /// The <see cref="Mark"/> for player two.
        /// </summary>
        private readonly Mark playerTwoMark;

        /// <summary>
        /// The user name of player one.
        /// </summary>
        private string playerOne;

        /// <summary>
        /// The user name of player you.
        /// </summary>
        private string playerTwo;

        /// <summary>
        /// If true the client is a player, otherwise false.
        /// </summary>
        private bool isPlayer;

        /// <summary>
        /// The player ID the server needs for game moves to validate you are a player, may be null.
        /// </summary>
        private string playerID;

        /// <summary>
        /// The unique game ID for the game on the server.
        /// </summary>
        private string gameID;

        /// <summary>
        /// The user name of the user whose turn it is.
        /// </summary>
        private string turnOf_UserName;

        /// <summary>
        /// True if the game has started, false otherwise. Is needed for the view model.
        /// </summary>
        private bool isStarted;

        /// <summary>
        /// True if the game is closed, false otherwise.
        /// </summary>
        private bool isClosed;

        /// <summary>
        /// <see cref="playerOne"/>
        /// </summary>
        public string PlayerOne
        {
            get
            {
                return this.playerOne;
            }
            private set
            {
                this.playerOne = value;
            }
        }


        /// <summary>
        /// <see cref="playerTwo"/>
        /// </summary>
        public string PlayerTwo
        {
            get
            {
                return this.playerTwo;
            }
            private set
            {
                this.playerTwo = value;
            }
        }

        /// <summary>
        /// <see cref="isPlayer"/>
        /// </summary>
        public bool IsPlayer
        {
            get
            {
                return this.isPlayer;
            }
            private set
            {
                this.isPlayer = value;
            }
        }

        /// <summary>
        /// <see cref="isClosed"/>
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return this.isClosed;
            }
            set
            {
                this.isClosed = value;
            }
        }

        /// <summary>
        /// <see cref="isStarted"/>
        /// </summary>
        public bool IsStarted
        {
            get
            {
                return this.isStarted;
            }
            set
            {
                this.isStarted = value;
            }
        }

        /// <summary>
        /// <see cref="gameID"/>
        /// </summary>
        public string GameID
        {
            get
            {
                return this.gameID;
            }
            private set
            {
                this.gameID = value;
            }
        }

        /// <summary>
        /// <see cref="playerID"/>
        /// </summary>
        public string PlayerID
        {
            get
            {
                return this.playerID;
            }
            private set
            {
                this.playerID = value;
            }
        }

        /// <summary>
        /// Initializes a new instanze of the <see cref="Game"/>.
        /// </summary>
        /// <param name="gameID">The game ID.</param>
        /// <param name="playerOne">The name of player one.</param>
        /// <param name="playerTwo">The name of player two.</param>
        /// <param name="isPlayer">True if the client is a player, false otherwise.</param>
        /// <param name="playerID">The player ID.</param>
        public Game(string gameID, string playerOne, string playerTwo, bool isPlayer, string playerID)
        {
            if (string.IsNullOrWhiteSpace(gameID))
                throw new ArgumentNullException(nameof(gameID), "The specified parameter must not be null!");

            if (string.IsNullOrWhiteSpace(playerOne))
                throw new ArgumentNullException(nameof(playerOne), "The specified parameter must not be null!");

            if (string.IsNullOrWhiteSpace(playerTwo))
                throw new ArgumentNullException(nameof(playerTwo), "The specified parameter must not be null!");

            if (string.IsNullOrWhiteSpace(playerID) && isPlayer)
                throw new ArgumentNullException(nameof(playerID), $"The specified parameter must not be null while {nameof(isPlayer)} is true!");


            this.playerOneMark = new Mark(System.Drawing.Color.Blue);
            this.playerTwoMark = new Mark(System.Drawing.Color.Red);
            this.GameBoard = new GameBoard();

            this.PlayerOne = playerOne;
            this.PlayerTwo = playerTwo;
            this.GameID = gameID;
            this.IsClosed = false;
            this.IsPlayer = isPlayer;

            if (this.IsPlayer)
            {
                this.PlayerID = playerID;
            }

            //TODO sollt noch geändert werden dass beides false ist theoretisch
            if (IsPlayer)
            {
                this.IsStarted = false;
            }
            else
            {
                this.IsStarted = true;
            }
        }

        /// <summary>
        /// Sets the players marks in the gameboard.
        /// </summary>
        /// <param name="playerName">The user name who makes the move.</param>
        /// <param name="column">The column in which the mark is placed.</param>
        /// <returns>The row where the mark is placed.</returns>
        public int SetMark(string playerName, int column)
        {
            int setRow = -1;
            SetMarkCommand command;

            if (playerName == playerOne)
            {
                command = new SetMarkCommand(this.playerOneMark, this.GameBoard, column, row =>
                {
                    setRow = row;
                });

                command.Execute();
            }
            else if (playerName == PlayerTwo)
            {
                command = new SetMarkCommand(this.playerTwoMark, this.GameBoard, column, row =>
                {
                    setRow = row;
                });

                command.Execute();
            }

            return setRow;
        }
    }
}
