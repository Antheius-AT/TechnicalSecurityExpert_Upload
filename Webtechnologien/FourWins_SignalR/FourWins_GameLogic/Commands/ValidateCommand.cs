//-----------------------------------------------------------------------
// <copyright file="ValidateCommand.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
namespace FourWins_GameLogic.Commands
{
    using System;
    using FourWins_GameLogic.GameLogicComponents;
    using FourWins_GameLogic.Interfaces;

    /// <summary>
    /// Represents a command that validates wheter a specific move is possible.
    /// </summary>
    public class ValidateCommand : IGameCommand
    {
        /// <summary>
        /// The callback to execute when the command execution terminates.
        /// </summary>
        private readonly Action<bool> callBack;

        /// <summary>
        /// The game board containing the current game state.
        /// </summary>
        private readonly GameBoard currentBoard;

        /// <summary>
        /// The column to check whether a valid move can be performed on this column.
        /// </summary>
        private readonly int column;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateCommand"/> class.
        /// </summary>
        /// <param name="callBack">The method to be executed after the command execution finished.</param>
        /// <param name="currentBoard">The game board containing the current game state.</param>
        /// <param name="column">The column to check for when validating whether a move is possible.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either the call back method or the game board are null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Is thrown if column is negative or if column is greater than amount of columns on the game board.
        /// </exception>
        public ValidateCommand(Action<bool> callBack, GameBoard currentBoard, int column)
        {
            if (callBack == null)
                throw new ArgumentNullException(nameof(callBack));

            if (currentBoard == null)
                throw new ArgumentNullException(nameof(currentBoard), "Current game board must not be null.");

            if (column < 0)
                throw new ArgumentOutOfRangeException(nameof(column), "Column must not be negative.");

            if (column > currentBoard.columns)
                throw new ArgumentOutOfRangeException(nameof(column), "Column must not be greater than columns on the board.");

            this.callBack = callBack;
            this.currentBoard = currentBoard;
            this.column = column;
        }

        /// <summary>
        /// Executes the validate command.
        /// </summary>
        public void Execute()
        {
            var isValidMove = !this.currentBoard.cells[0, this.column].IsLoaded;
            this.callBack(isValidMove);
        }
    }
}
