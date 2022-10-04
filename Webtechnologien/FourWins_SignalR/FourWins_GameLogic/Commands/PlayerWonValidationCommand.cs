//-----------------------------------------------------------------------
// <copyright file="PlayerWonValidationCommand.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman, Christian Giessrigl.</author>
//-----------------------------------------------------------------------
namespace FourWins_GameLogic.Commands
{
    using System;
    using FourWins_GameLogic.GameLogicComponents;
    using FourWins_GameLogic.Interfaces;

    /// <summary>
    /// Represent a command that checks whether a player has won the game.
    /// </summary>
    public class PlayerWonValidationCommand : IGameCommand
    {
        /// <summary>
        /// The game board containing the game cells.
        /// </summary>
        private readonly GameBoard board;

        /// <summary>
        /// The method to invoke when the command execution is finished.
        /// </summary>
        private readonly Action<bool> callBack;

        /// <summary>
        /// The last mark that has been put on the game board.
        /// </summary>
        private readonly Mark lastMark;

        /// <summary>
        /// The row at which the last mark has been put in the game board.
        /// </summary>
        private readonly int row;

        /// <summary>
        /// The column at which the last mark has been put in the game board.
        /// </summary>
        private readonly int column;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerWonValidationCommand"/> class.
        /// </summary>
        /// <param name="board">The game board.</param>
        /// <param name="lastMark">The last mark that has been put on the game board.</param>
        /// <param name="row">The row in which the last mark has been put into.</param>
        /// <param name="column">The column in which the last mark has been put into.</param>
        /// <param name="callBack">The call back method to be invoked when the command has executed.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either the board or the call back are null.
        /// </exception>
        public PlayerWonValidationCommand(GameBoard board, Mark lastMark, int row, int column, Action<bool> callBack)
        {
            if (board == null)
                throw new ArgumentNullException(nameof(board), "Game board must not be null.");

            if (callBack == null)
                throw new ArgumentNullException(nameof(callBack), "Call back must not be null.");

            this.board = board;
            this.lastMark = lastMark;
            this.row = row;
            this.column = column;
            this.callBack = callBack;
        }

        /// <summary>
        /// Executes the validation if the last mark produces a win condition.
        /// </summary>
        public void Execute()
        {
            bool playerHasWon = this.CheckHorizontalLineForWin() || 
                                this.CheckVerticalLineForWin() || 
                                this.CheckDiagonalLeftToRightForWin() || 
                                this.CheckDiagonalRightToLeftForWin();

            this.callBack(playerHasWon);
        }

        /// <summary>
        /// Checks the row for win.
        /// </summary>
        /// <returns>True if at least 4 marks of the same color are adjacent in row. False if not.</returns>
        private bool CheckHorizontalLineForWin()
        {
            int count = 0;

            for (int i = this.column; i < this.board.columns; i++)
            {
                if (this.board.cells[row, i].IsLoaded)
                {
                    if (this.board.cells[row, i].PlayerMark.Color == this.lastMark.Color)
                    {
                        count++;
                        continue;
                    }
                }

                break;
            }

            for (int i = this.column; i >= 0; i--)
            {
                if (this.board.cells[row, i].IsLoaded)
                {
                    if (this.board.cells[row, i].PlayerMark.Color == this.lastMark.Color)
                    {
                        count++;
                        continue;
                    }
                }

                break;
            }

            count--;

            if (count >= 4)
                return true;

            return false;
        }

        /// <summary>
        /// Checks the column for win.
        /// </summary>
        /// <returns>True if at least 4 marks of the same color are adjacent in column. False if not.</returns>
        private bool CheckVerticalLineForWin()
        {
            int count = 0;

            for (int i = this.row; i < this.board.rows; i++)
            {
                if (this.board.cells[i, column].IsLoaded)
                {
                    if (this.board.cells[i, column].PlayerMark.Color == this.lastMark.Color)
                    {
                        count++;
                        continue;
                    }
                }

                break;
            }

            for (int i = this.row; i >= 0; i--)
            {
                if (this.board.cells[i, column].IsLoaded)
                {
                    if (this.board.cells[i, column].PlayerMark.Color == this.lastMark.Color)
                    {
                        count++;
                        continue;
                    }
                }

                break;
            }

            count--;

            if (count >= 4)
                return true;

            return false;
        }

        /// <summary>
        /// Checks the left upper to right lower diagonal for win.
        /// </summary>
        /// <returns>True if at least 4 marks of the same color are adjacent in left to right diagonal line. False if not.</returns>
        private bool CheckDiagonalLeftToRightForWin()
        {
            int count = 0;

            for (int i = this.row, j = this.column; i >= 0 && j >= 0; i--, j--)
            {
                if (this.board.cells[i, j].IsLoaded)
                {
                    if (this.board.cells[i, j].PlayerMark.Color == this.lastMark.Color)
                    {
                        count++;
                        continue;
                    }
                }

                break;
            }

            for (int i = this.row, j = this.column; i < this.board.rows && j < this.board.columns; i++, j++)
            {
                if (this.board.cells[i, j].IsLoaded)
                {
                    if (this.board.cells[i, j].PlayerMark.Color == this.lastMark.Color)
                    {
                        count++;
                        continue;
                    }
                }

                break;
            }

            count--;

            if (count >= 4)
                return true;

            return false;
        }

        /// <summary>
        /// Checks the right upper to left lower diagonal for win.
        /// </summary>
        /// <returns>True if at least 4 marks of the same color are adjacent in right to left diagonal line. False if not.</returns>
        private bool CheckDiagonalRightToLeftForWin()
        {
            int count = 0;

            for (int i = this.row, j = this.column; i >= 0 && j < this.board.columns; i--, j++)
            {
                if (this.board.cells[i, j].IsLoaded)
                {
                    if (this.board.cells[i, j].PlayerMark.Color == this.lastMark.Color)
                    {
                        count++;
                        continue;
                    }
                }

                break;
            }

            for (int i = this.row, j = this.column; i < this.board.rows && j >= 0; i++, j--)
            {
                if (this.board.cells[i, j].IsLoaded)
                {
                    if (this.board.cells[i, j].PlayerMark.Color == this.lastMark.Color)
                    {
                        count++;
                        continue;
                    }
                }

                break;
            }

            count--;

            if (count >= 4)
                return true;

            return false;
        }
    }
}
