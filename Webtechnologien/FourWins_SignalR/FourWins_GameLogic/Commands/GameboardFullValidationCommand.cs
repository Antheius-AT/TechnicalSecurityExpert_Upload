//-----------------------------------------------------------------------
// <copyright file="GameboardFullValidationCommand.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace FourWins_GameLogic.Commands
{
    using System;
    using FourWins_GameLogic.GameLogicComponents;
    using FourWins_GameLogic.Interfaces;

    /// <summary>
    /// This class represent a command that validates whether the game is over.
    /// </summary>
    public class GameboardFullValidationCommand : IGameCommand
    {
        /// <summary>
        /// The game board containing all of the game cells.
        /// </summary>
        private readonly GameBoard board;

        /// <summary>
        /// The call back that is to be invoked after the command has executed.
        /// </summary>
        private readonly Action<bool> callBack;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameboardFullValidationCommand"/> class.
        /// </summary>
        /// <param name="board">The game board containing the cells.</param>
        /// <param name="callBack">The call back method that is to be invoked after execution of the command.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either the game board or the call back method are null.
        /// </exception>
        public GameboardFullValidationCommand(GameBoard board, Action<bool> callBack)
        {
            if (board == null)
                throw new ArgumentNullException(nameof(board), "Board must not be null.");

            if (callBack == null)
                throw new ArgumentNullException(nameof(callBack), "Call back must not be null.");

            this.board = board;
            this.callBack = callBack;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        public void Execute()
        {
            bool isFull = true;

            for (int i = 0; i < this.board.columns; i++)
            {
                if (this.board.cells[0, i].IsLoaded)
                    continue;

                isFull = false;
                break;
            }

            this.callBack(isFull);
        }
    }
}
