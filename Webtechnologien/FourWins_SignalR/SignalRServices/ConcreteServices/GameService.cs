//-----------------------------------------------------------------------
// <copyright file="GameService.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Christian Giessrigl.</author>
//-----------------------------------------------------------------------
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ServicesTests")]

namespace SignalRServices.ConcreteServices
{
    using FourWins_GameLogic.Commands;
    using FourWins_GameLogic.GameLogicComponents;
    using SharedData.GameData;
    using SignalRServices.Interfaces.ServiceInterfaces.GameServices;
    using SignalRServices.ServiceData;
    using SignalRServices.ServiceData.GameServiceData;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading.Tasks;

    public class GameService : IGameService
    {
        /// <summary>
        /// The list of running games.
        /// </summary>
        internal readonly ConcurrentDictionary<string, GameData> games;

        private readonly object locker;

        /// <summary>
        /// Initializes a new instance of the<see cref="GameService"/> class.
        /// </summary>
        public GameService()
        {
            this.games = new ConcurrentDictionary<string, GameData>();
            this.locker = new object();
        }

        /// <summary>
        /// Stores the game in the list of running games.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <param name="data">The information of the game.</param>
        /// <exception cref="ArgumentNullException">If gameID is null or whitespace.</exception>
        /// /// <exception cref="ArgumentNullException">If data is null.</exception>
        /// <returns></returns>
        public Task StoreGame(string gameID, GameData data)
        {
            if (string.IsNullOrWhiteSpace(gameID))
                throw new ArgumentNullException(nameof(gameID), "GameID must not be null or whitespace.");

            if (data == null)
                throw new ArgumentNullException(nameof(data), "Data must not be null.");

            this.games.TryAdd(gameID, data);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Performs a full validated game move.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <param name="playerID">The ID to verify as a player.</param>
        /// <param name="playerName">The name of the player.</param>
        /// <param name="column">The column to set the mark in.</param>
        /// <exception cref="ArgumentNullException"> If gameID is not found.</exception>
        /// <exception cref="InvalidOperationException"> If the players name is not the current players name. Or the player ID is invalid.</exception>
        /// <exception cref="ArgumentNullException"> Is thrown if column has a negative value. </exception>
        public Task<GameMoveData> MakeGameMoveAsync(string gameID, string playerID, string playerName, int column)
        {
            if (column < 0)
                throw new ArgumentOutOfRangeException(nameof(column), "Column must have a positiv value.");

            return Task.Run(() =>
            {
                if (!this.games.TryGetValue(gameID, out GameData game))
                    throw new ArgumentNullException($"No game with ID: {gameID} found.");

                if (game.CurrentPlayer != playerName)
                    throw new InvalidOperationException($"{playerName} is not the current player!");

                if (game.PlayerID != playerID)
                    throw new InvalidOperationException($"{playerID} is not the valid player ID.");

                bool isValid = this.ValidateMove(game, column);
               
                if (isValid)
                {
                    CompletedMove placedMark = this.SetMark(game, playerName, column);
                    bool isWinner = this.VerifyGameWonCondition(game, placedMark);
                    bool isFull = this.CheckGameBoardFullCondition(game.GameBoard);

                    return new GameMoveData(gameID, playerName, isValid, isFull, isWinner, column);
                }

                return new GameMoveData(gameID, playerName, isValid, false, false, column);
            });
        }

        /// <summary>
        /// Validates if the move is doable. 
        /// </summary>
        /// <param name="gameData">The data of the game.</param>
        /// <param name="playerName">The name of the player who makes the move.</param>
        /// <param name="column">The column </param>
        /// <returns>True if move is valid. False if not.</returns>
        /// <exception cref = "ArgumentNullException" >
        /// Is thrown if game data is null.
        /// </exception>
        private bool ValidateMove(GameData gameData, int column)
        {
            if (gameData == null)
                throw new ArgumentNullException(nameof(gameData), "Game must not be null.");

            bool valid = false;
            var validation = new ValidateCommand(isValid =>
            {
                valid = isValid;

            }, gameData.GameBoard, column);

            validation.Execute();

            return valid;
        }

        /// <summary>
        /// Sets a mark in the gameboard based on a specified column.
        /// </summary>
        /// <param name="gameData">The information of the game.</param>
        /// <param name="playerName">The name of the player.</param>
        /// <param name="column">The column of the game board the mark will be put in.</param>
        /// <returns>The placed mark and the row of the gameboard where it was placed.</returns>
        /// <exception cref = "ArgumentNullException" >
        /// Is thrown if game data is null.
        /// </exception>
        private CompletedMove SetMark(GameData gameData, string playerName, int column)
        {
            if (gameData == null)
                throw new ArgumentNullException(nameof(gameData), "Game must not be null.");

            Mark playerMark = null;

            if (gameData.Players[0] == playerName)
                playerMark = new Mark(Color.Red);
            else
                playerMark = new Mark(Color.Green);

            int row = -1;
            var move = new SetMarkCommand(playerMark, gameData.GameBoard, column, currentRow =>
            {
                row = currentRow;
            });

            move.Execute();

            gameData.ValidGameMovesDone.Enqueue(new QueuedGameMove(playerName, column));

            // Bist du sicher dass hier nicht die column stehn sollte, weil sich die row automatisch ergibt?
            return new CompletedMove(playerMark, column, row);
        }

        /// <summary>
        /// Checks if one of the players has won based on the last placed mark.
        /// </summary>
        /// <param name="game">The information of the game.</param>
        /// <param name="move">The completed move containing information about the last move that was being played.</param>
        /// <returns>Whether there is a winner after the last move.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if game data or move are null.
        /// </exception>
        private bool VerifyGameWonCondition(GameData game, CompletedMove move)
        {
            if (game == null)
                throw new ArgumentNullException(nameof(game), "Game must not be null.");

            if (move == null)
                throw new ArgumentNullException(nameof(move), "Move must not be null.");

            bool isWinner = false;

            var winCondition = new PlayerWonValidationCommand(game.GameBoard, move.PlayerMark, move.Row, move.Column, hasWon =>
            {
                isWinner = hasWon;
            });

            winCondition.Execute();
            return isWinner;
        }

        /// <summary>
        /// Checks if the game board is full.
        /// </summary>
        /// <param name="gameBoard">The specified game board.</param>
        /// <returns>True if the gameboard is full otherwise false.</returns>
        private bool CheckGameBoardFullCondition(GameBoard gameBoard)
        {
            if (gameBoard == null)
                throw new ArgumentNullException(nameof(gameBoard), "Gameboard must not be null.");

            bool result = false;

            var fullCondition = new GameboardFullValidationCommand(gameBoard, isFull =>
            {
                result = isFull;
            });

            fullCondition.Execute();
            return result;
        }

        /// <summary>
        /// Terminates an existing game.
        /// </summary>
        /// <param name="gameID">The id of the game to terminate.</param>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        public Task<bool> TerminateGameAsync(string gameID, string playerID)
        {
            return Task.Run(() =>
            {
                if (!this.games.TryGetValue(gameID, out GameData game))
                    throw new ArgumentNullException($"No game with ID: {gameID} found.");

                if (game.PlayerID == playerID)
                {
                    this.games.TryRemove(gameID, out _);
                    return true;
                }

                return false;
            });
        }

        /// <summary>
        /// Checks if the player is a player of this game.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <param name="playerID">The ID to verify as a player of the game.</param>
        /// <param name="playerName">The name of the player.</param>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        public Task<bool> VerifyPlayerAsync(string gameID, string playerID, string playerName)
        {
            return Task.Run(() =>
            {
                if (!this.games.TryGetValue(gameID, out GameData game))
                    throw new ArgumentNullException($"No game with ID: {gameID} found.");

                if (game.PlayerID != playerID)
                    return false;

                if (string.IsNullOrWhiteSpace(playerName))
                    throw new ArgumentNullException(nameof(playerName), "Player must not be null or whitespace.");

                lock (locker)
                {
                    if (string.IsNullOrWhiteSpace(game.Players[0]))
                    {
                        game.CurrentPlayer = playerName;
                        game.Players[0] = playerName;
                        return true;
                    }

                    for (int i = 1; i < game.Players.Length; i++)
                    {
                        if (string.IsNullOrWhiteSpace(game.Players[i]))
                        {
                            game.Players[i] = playerName;
                            return true;
                        }
                    }
                    return false;
                }
            });
        }

        /// <summary>
        /// Checks if required amount of players has verified.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <returns>True if game is full. False if not.</returns>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        public Task<bool> IsGameFull(string gameID)
        {
            return Task.Run(() =>
            {
                if (!this.games.TryGetValue(gameID, out GameData game))
                    throw new ArgumentNullException($"No game with ID: {gameID} found.");

                for (int i = 0; i < game.Players.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(game.Players[i]))
                    {
                        return false;
                    }
                }

                return true;
            });
        }

        /// <summary>
        /// Gets the current game state asynchronously.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <returns>The current game state.</returns>
        /// <exception cref="ArgumentException">
        /// Is thrown if there was no game with the specified ID found.
        /// </exception>
        public Task<GameStateData> GetCurrentGameStateAsync(string gameID)
        {
            return Task.Run(() =>
            {
                if (!this.games.TryGetValue(gameID, out GameData game))
                    throw new ArgumentNullException($"No game with ID: {gameID} found.");

                return new GameStateData() 
                { 
                    GameID = gameID,
                    ValidGameMovesDone = game.ValidGameMovesDone, 
                    Players = game.Players, 
                    CurrentPlayer = game.CurrentPlayer
                };
            });
        }

        /// <summary>
        /// Checks if it is the specified players turn.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <param name="playerName">The name of the player.</param>
        /// <returns>True if it is the specified players turn, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        public Task<bool> IsPlayersTurnAsync(string gameID, string playerName)
        {
            return Task.Run<bool>(() =>
            {
                if (!this.games.TryGetValue(gameID, out GameData game))
                    throw new ArgumentNullException($"No game with ID: {gameID} found.");

                if (game.CurrentPlayer == playerName)
                    return true;

                return false;
            });
        }

        /// <summary>
        /// Checks if the specified player is a player of the game.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <param name="playerName">The name of the player.</param>
        /// <returns>True if the specified player is a player of the game, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        public Task<bool> IsPlayerOfGameAsync(string gameID, string playerName)
        {
            return Task.Run<bool>(() =>
            {
                if (!this.games.TryGetValue(gameID, out GameData game))
                    throw new ArgumentNullException($"No game with ID: {gameID} found.");

                foreach (var player in game.Players)
                {
                    if (player == playerName)
                        return true;
                }

                return false;
            });
        }

        /// <summary>
        /// Change the current player to the next player in the list.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        public Task ChangeCurrentPlayerAsync(string gameID)
        {
            if (!this.games.TryGetValue(gameID, out GameData game))
                throw new ArgumentNullException($"No game with ID: {gameID} found.");

            if (game.CurrentPlayer == string.Empty)
            {
                game.CurrentPlayer = game.Players[0];
                return Task.CompletedTask;
            }

            for (int i = 0; i < game.Players.Length; i++)
            {
                try
                {
                    if (game.Players[i] == game.CurrentPlayer)
                    {
                        game.CurrentPlayer = game.Players[i + 1];
                        return Task.CompletedTask;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    game.CurrentPlayer = game.Players[0];
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the games turn time in seconds.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <returns>The games turn time in seconds.</returns>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        public Task<int> GetGameTurnTime(string gameID)
        {
            return Task.Run(() =>
            {
                if (!this.games.TryGetValue(gameID, out GameData game))
                    throw new ArgumentNullException($"No game with ID: {gameID} found.");

                return game.TurnTime;
            });
        }

        /// <summary>
        /// Gets the games current player name.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <returns>The games current player name.</returns>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        public Task<string> GetCurrentPlayer(string gameID)
        {
            return Task.Run(() =>
            {
                if (!this.games.TryGetValue(gameID, out GameData game))
                    throw new ArgumentNullException($"No game with ID: {gameID} found.");

                return game.CurrentPlayer;
            });
        }

        /// <summary>
        /// Gets the games list of players that are not the currentPlayer.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <returns>The games list of players that are not the currentPlayer.</returns>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        public Task<List<string>> GetNonCurrentPlayers(string gameID)
        {
            return Task.Run(() =>
            {
                if (!this.games.TryGetValue(gameID, out GameData game))
                    throw new ArgumentNullException($"No game with ID: {gameID} found.");

                List<string> players = new List<string>();

                for (int i = 0; i < game.Players.Length; i++)
                {
                    if (game.Players[i] != game.CurrentPlayer && !string.IsNullOrWhiteSpace(game.Players[i]))
                        players.Add(game.Players[i]);
                }

                return players;
            });
        }
    }
}
