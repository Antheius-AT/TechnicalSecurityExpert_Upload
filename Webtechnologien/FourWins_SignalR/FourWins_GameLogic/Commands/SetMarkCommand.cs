//-----------------------------------------------------------------------
// <copyright file="SetMarkCommand.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Christian Giessrigl.</author>
//-----------------------------------------------------------------------
namespace FourWins_GameLogic.Commands
{
    using System;
    using FourWins_GameLogic.Exceptions;
    using FourWins_GameLogic.GameLogicComponents;
    using FourWins_GameLogic.Interfaces;

    /// <summary>
    /// Represents a command that places a mark on the game board.
    /// </summary>
    public class SetMarkCommand : IGameCommand
    {
        /// <summary>
        /// The game board containing all the game cells.
        /// </summary>
        private readonly GameBoard gameBoard;

        /// <summary>
        /// The amount of columns on the game board.
        /// </summary>
        private readonly int column;

        /// <summary>
        /// The mark to be placed on the game board.
        /// </summary>
        private readonly Mark mark;

        /// <summary>
        /// The callback to execute when the command execution terminates.
        /// </summary>
        private readonly Action<int> callBack;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetMarkCommand"/> class.
        /// </summary>
        /// <param name="mark">The mark to set.</param>
        /// <param name="gameBoard">The game board containing all cells.</param>
        /// <param name="column">The column in which to place the mark.</param>
        /// <param name="callBack">The method to be executed after the command execution finished.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if the mark or game board are null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Is thrown if the column is negative or greater than the amount of columns.
        /// </exception>
        public SetMarkCommand(Mark mark, GameBoard gameBoard, int column, Action<int> callback)
        {
            if (mark == null)
                throw new ArgumentNullException(nameof(mark), "Mark to place must not be null.");

            if (gameBoard == null)
                throw new ArgumentNullException(nameof(gameBoard), "Game board must not be null.");

            if (column < 0)
                throw new ArgumentOutOfRangeException(nameof(column), "column must not be null.");

            if (column > gameBoard.columns)
                throw new ArgumentOutOfRangeException(nameof(column), "column must not be greater than amount of columns on the game board");

            this.gameBoard = gameBoard;
            this.column = column;
            this.mark = mark;
            this.callBack = callback;
        }

        /// <summary>
        /// Executes the game board changes.
        /// </summary>
        /// <exception cref="ColumnFullyLoadedException">
        /// Is thrown if the column specified is already fully loaded.
        /// </exception>
        public void Execute()
        {
            if (this.gameBoard.cells[0, column].IsLoaded)
                throw new ColumnFullyLoadedException($"Column {this.column} is already full.");

            this.callBack(this.gameBoard.SetMark(this.mark, this.column));
        }
    }
}
