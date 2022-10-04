//-----------------------------------------------------------------------
// <copyright file="IGameService.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Christian Giessrigl.</author>
//-----------------------------------------------------------------------
namespace SignalRServices.Interfaces.ServiceInterfaces.GameServices
{
    using SharedData.GameData;
    using SignalRServices.EventArgs;
    using SignalRServices.ServiceData;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGameService
    {
        /// <summary>
        /// Stores the game in the list of running games.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <param name="data">The information of the game.</param>
        /// <exception cref="ArgumentNullException">If gameID is null or whitespace.</exception>
        /// <exception cref="ArgumentNullException">If data is null.</exception>
        /// <returns></returns>
        Task StoreGame(string gameID, GameData data);

        /// <summary>
        /// Terminates an existing game.
        /// </summary>
        /// <param name="gameID">The id of the game to terminate.</param>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        Task<bool> TerminateGameAsync(string gameID, string playerID);

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
        Task<GameMoveData> MakeGameMoveAsync(string gameID, string playerID, string playerName, int column);

        /// <summary>
        /// Checks if the player is a player of this game.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <param name="playerID">The ID to verify as a player of the game.</param>
        /// <param name="playerName">The name of the player.</param>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        Task<bool> VerifyPlayerAsync(string gameID, string playerID, string playerName);

        /// <summary>
        /// Gets the current game state asynchronously.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <returns>The current game state.</returns>
        /// <exception cref="ArgumentException">
        /// Is thrown if there was no game with the specified ID found.
        /// </exception>
        Task<GameStateData> GetCurrentGameStateAsync(string gameID);

        /// <summary>
        /// Checks if it is the specified players turn.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <param name="playerName">The name of the player.</param>
        /// <returns>True if it is the specified players turn, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        Task<bool> IsPlayersTurnAsync(string gameID, string playerName);

        /// <summary>
        /// Checks if the specified player is a player of the game.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <param name="playerName">The name of the player.</param>
        /// <returns>True if the specified player is a player of the game, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        Task<bool> IsPlayerOfGameAsync(string gameID, string playerName);

        /// <summary>
        /// Change the current player to the next player in the list.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        Task ChangeCurrentPlayerAsync(string gameID);

        /// <summary>
        /// Checks if required amount of players has verified.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <returns>True if game is full. False if not.</returns>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        Task<bool> IsGameFull(string gameID);

        /// <summary>
        /// Gets the games turn time in seconds.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <returns>The games turn time in seconds.</returns>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        Task<int> GetGameTurnTime(string gameID);

        /// <summary>
        /// Gets the games current player name.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <returns>The games current player name.</returns>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        Task<string> GetCurrentPlayer(string gameID);

        /// <summary>
        /// Gets the games list of players that are not the currentPlayer.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <returns>The games list of players that are not the currentPlayer.</returns>
        /// <exception cref="ArgumentNullException">If gameID is not found.</exception>
        Task<List<string>> GetNonCurrentPlayers(string gameID);
    }
}
