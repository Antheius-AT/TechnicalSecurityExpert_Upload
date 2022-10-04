using System;
using System.Drawing;
using FourWins_GameLogic.Commands;

namespace FourWins_GameLogic
{
    class Program
    {
        //private static GameBoard board = new GameBoard();
        //private static Mark mark = new Mark(Color.Red);
        //private static Mark anotherMark = new Mark(Color.Blue);

        static void Main()
        {

            // When getting data from user we need to subtract one, to compensate for index.
            // 4 actually targets the 5th row, as we start with 0. Means, if the client sends the server "Please mark column x", 
            // The server needs to mark column x - 1 instead.
            //var markCommand = new SetMarkCommand(mark, board, 4);
            //var anotherMarkCommand = new SetMarkCommand(anotherMark, board, 4);
            //var validateCommand = new ValidateCommand(ValidateMoveCallback, board, 4);
            //var playerWonCommand = new PlayerWonValidationCommand(board, HasPlayerWonCallback);

            //markCommand.Execute();
            //anotherMarkCommand.Execute();
            //anotherMarkCommand.Execute();
            //markCommand.Execute();
            //validateCommand.Execute();
            //markCommand.Execute();
            //markCommand.Execute();
            //validateCommand.Execute();

            //Console.ReadLine();
        }

        //private static void ValidateMoveCallback(bool isValid)
        //{
        //    if (isValid)
        //        Console.WriteLine("Return true to client.");
        //    else
        //        Console.WriteLine("return false to client.");
        //}

        //private static void HasPlayerWonCallback(bool hasPlayerWon, Color colorWon)
        //{
        //    if (hasPlayerWon)
        //    {
        //        Console.WriteLine($"Signal client that player {colorWon} has won.");
        //    }
        //    else
        //    {
        //        var command = new GameboardFullValidationCommand(board, GameboardFullCallback);
        //        command.Execute();
        //    }
        //}

        //private static void GameboardFullCallback(bool isFull)
        //{
        //    if (isFull)
        //        Console.WriteLine("Return to player that game is over because board is full.");
        //    else
        //        Console.WriteLine("Return to player that game can continue because board has still space left");
        //}
    }
}
