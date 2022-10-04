//-----------------------------------------------------------------------
// <copyright file="GameBoard.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
namespace FourWins_GameLogic.GameLogicComponents
{
    using FourWins_GameLogic.Exceptions;
    using System;

    /// <summary>
    /// This class represents the game board.
    /// </summary>
    public class GameBoard
    {
        /// <summary>
        /// The amount of columns on the board.
        /// </summary>
        public readonly int columns;

        /// <summary>
        /// The amount of rows on the board.
        /// </summary>
        public readonly int rows;

        /// <summary>
        /// A two dimensional array of game cells on the game board.
        /// </summary>
        public readonly GameCell[,] cells;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameBoard"/> class.
        /// </summary>
        public GameBoard()
        {
            this.columns = 7;
            this.rows = 6;
            this.cells = new GameCell[6, 7];

            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    cells[i, j] = new GameCell();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameBoard"/> class.
        /// </summary>
        /// <param name="columns">The amount of columns on the game board.</param>
        /// <param name="rows">The amount of rows on the game board.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Is thrown if either of the parameters is negative.
        /// </exception>
        public GameBoard(int columns, int rows)
        {
            if (columns < 0)
                throw new ArgumentOutOfRangeException(nameof(columns), "Value for columns must not be negative.");

            if (rows < 0)
                throw new ArgumentOutOfRangeException(nameof(rows), "Value for rows must not be negative.");

            this.columns = columns;
            this.rows = rows;
            this.cells = new GameCell[rows, columns];

            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    cells[i, j] = new GameCell();
                }
            }
        }

        /// <summary>
        /// Places a mark onto the game board.
        /// </summary>
        /// <param name="mark">The mark to place.</param>
        /// <param name="column">The column in which to place the mark.</param>
        /// <exception cref="ColumnFullyLoadedException">
        /// Is thrown if the mark can`t be placed into the specified column, because it is fully loaded.
        /// </exception>
        public int SetMark(Mark mark, int column)
        {
            if (mark == null)
                throw new ArgumentNullException(nameof(mark), "Mark must not be null.");

            if (column < 0 || column > this.columns)
                throw new ArgumentOutOfRangeException(nameof(column), "Column must not be negative, or greater than amount of columns");

            if (this.cells[0, column].IsLoaded)
                throw new ColumnFullyLoadedException("Mark can not be placed in the specified column, because column is already full.");

            for (int i = this.rows - 1; i >= 0; i--)
            {
                if (this.cells[i, column].IsLoaded)
                    continue;

                this.cells[i, column].PlayerMark = mark;
                return i;
            }

            return -1;
        }
    }
}
