//-----------------------------------------------------------------------
// <copyright file="GameLogicTests.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Giessrigl Christian.</author>
//-----------------------------------------------------------------------
namespace ServicesTests
{
    using FourWins_GameLogic.Commands;
    using FourWins_GameLogic.GameLogicComponents;
    using NUnit.Framework;
    using System;
    using System.Collections;
    using System.Drawing;

    public class GameLogicTests
    {
        [Test, TestCaseSource(typeof(GameLogicTestData), "GetValidateSetMarkCommandData")]
        public bool Validates_Set_Mark_Command(GameBoard board, int column)
        {
            bool valid = false;

            try
            {
                ValidateCommand command = new ValidateCommand(isValid =>
                {
                    valid = isValid;
                }, board, column);

                command.Execute();
            }
            catch (Exception)
            {
                return false;
            }

            return valid;
        }

        [Test, TestCaseSource(typeof(GameLogicTestData), "GetSetMarkCommandData")]
        public int Sets_Marks_Returns_Row_Of_Last_Mark_Set(GameBoard board, int[] columns)
        {
            for (int i = 0; i < columns.Length - 1; i++)
            {
                try
                {
                    SetMarkCommand command = new SetMarkCommand(new Mark(Color.Red), board, columns[i], row =>
                    {

                    });

                    command.Execute();
                }
                catch (Exception)
                {
                    return -1;
                }
            }

            int resultRow = -1;
            try
            {
                SetMarkCommand command = new SetMarkCommand(new Mark(Color.Red), board, columns[^1], row =>
                {
                    resultRow = row;
                });

                command.Execute();
            }
            catch (Exception)
            {
                return -1;
            }


            return resultRow;
        }

        [Test, TestCaseSource(typeof(GameLogicTestData), "GetGameboardFullValidationData")]
        public bool Validates_Gameboard_Full_Command_Returns_If_Gameboard_Is_Full(GameBoard board, int[] columns)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                SetMarkCommand setmark = new SetMarkCommand(new Mark(Color.Red), board, columns[i], row =>
                {

                });

                setmark.Execute();
            }

            bool gameboardFull = false;

            GameboardFullValidationCommand command = new GameboardFullValidationCommand(board, isFull =>
            {
                gameboardFull = isFull;
            });

            command.Execute();
               
            return gameboardFull;
        }

        [Test, TestCaseSource(typeof(GameLogicTestData), "GetPlayerWonValidationData")]
        public bool Validates_If_Player_Won_The_Game_Returns_If_Player_Wins_With_Last_Set_Mark(GameBoard board, int[] columns)
        {
            bool hasWon = false;

            for (int i = 0; i < columns.Length; i++)
            {
                Mark mark = null;
                if (i % 2 == 0)
                    mark = new Mark(Color.Red);
                else
                    mark = new Mark(Color.Green);

                SetMarkCommand setmark = new SetMarkCommand(mark, board, columns[i], row =>
                {
                    PlayerWonValidationCommand command = new PlayerWonValidationCommand(board, mark, row, columns[i], playerWon => 
                    {
                        hasWon = playerWon;
                    });

                    command.Execute();
                });

                setmark.Execute();
            }

            return hasWon;
        }
    }

    public static class GameLogicTestData
    {
        /// <summary>
        /// Params: (Gameboard: current gameboard, int: column where you want to set a mark in the gameboard)
        /// </summary>
        public static IEnumerable GetValidateSetMarkCommandData
        {
            get
            {
                yield return new TestCaseData(new GameBoard(), 0).Returns(true);
                yield return new TestCaseData(new GameBoard(), 6).Returns(true);
                yield return new TestCaseData(new GameBoard(), -1).Returns(false);
                yield return new TestCaseData(new GameBoard(), 7).Returns(false);

            }
        }

        /// <summary>
        /// Params: (Gameboard: current gameboard, int[]: columns where marks will be set in the gameboard)
        /// </summary>
        public static IEnumerable GetSetMarkCommandData
        {
            get
            {
                yield return new TestCaseData(new GameBoard(), new int[] { 0 }).Returns(5);
                yield return new TestCaseData(new GameBoard(), new int[] { 0, 0 }).Returns(4);
                yield return new TestCaseData(new GameBoard(), new int[] { 0, 1 }).Returns(5);
                yield return new TestCaseData(new GameBoard(), new int[] { 0, 1, 1 }).Returns(4);
                yield return new TestCaseData(new GameBoard(), new int[] { 0, 1, 1, 0, 0 }).Returns(3);

                yield return new TestCaseData(new GameBoard(), new int[] { 0, 0, 0 ,0 ,0 ,0 ,0 ,0 }).Returns(-1);
                yield return new TestCaseData(new GameBoard(), new int[] { -1 }).Returns(-1);
                yield return new TestCaseData(new GameBoard(), new int[] { 8 }).Returns(-1);
            }
        }

        /// <summary>
        /// Params: (Gameboard: current gameboard, int: column where marks will be set in the gameboard)
        /// </summary>
        public static IEnumerable GetGameboardFullValidationData
        {
            get
            {
                yield return new TestCaseData(new GameBoard(2, 2), new int[] { 0, 0, 1, 1}).Returns(true);

                yield return new TestCaseData(new GameBoard(2, 2), new int[] { 0, 0, 1 }).Returns(false);
                yield return new TestCaseData(new GameBoard(2, 2), new int[] { 1, 1, 0 }).Returns(false);
                yield return new TestCaseData(new GameBoard(), new int[] { 0, 0 }).Returns(false);
            }
        }

        /// <summary>
        /// Reminder: Player "changes" each turn! Params: (Gameboard: current gameboard, int: column where marks will be set in the gameboard).
        /// </summary>
        public static IEnumerable GetPlayerWonValidationData
        {
            get
            {
                yield return new TestCaseData(new GameBoard(), new int[] { 0, 1, 0, 1, 0, 1, 0 }).Returns(true);
                yield return new TestCaseData(new GameBoard(), new int[] { 0, 0, 1, 1, 2, 2, 3 }).Returns(true);
                yield return new TestCaseData(new GameBoard(), new int[] { 0, 1, 1, 2, 2, 3, 2, 3, 3, 4, 3 }).Returns(true);
                yield return new TestCaseData(new GameBoard(), new int[] { 0, 0, 1, 0, 0, 1, 1, 2, 2, 4, 3 }).Returns(true);

                yield return new TestCaseData(new GameBoard(), new int[] { 0, 0, 0, 0 }).Returns(false);
                yield return new TestCaseData(new GameBoard(), new int[] { 0, 0, 0, 0, 1, 1, 1 }).Returns(false);
                yield return new TestCaseData(new GameBoard(), new int[] { 0, 0, 0, 0, 1, 1, 1, 2, 2 }).Returns(false);
                yield return new TestCaseData(new GameBoard(), new int[] { 0, 0, 0, 0 }).Returns(false);
            }
        }
    }
}
