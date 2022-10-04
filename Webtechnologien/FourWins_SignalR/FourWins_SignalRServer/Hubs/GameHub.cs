//-----------------------------------------------------------------------
// <copyright file="GameHub.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Christian Giessrigl.</author>
//-----------------------------------------------------------------------
namespace FourWins_SignalRServer.Hubs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using FourWins_SignalRServer.ClientContract;
    using FourWins_SignalRServer.HubData;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.Extensions.Logging;
    using SharedData.Exceptions;
    using SharedData.SharedHubData.Interfaces;
    using SignalRServices.EventArgs;
    using SignalRServices.Interfaces.ServiceInterfaces.GameServices;
    using SignalRServices.Interfaces.ServiceInterfaces.SharedServices;
    using SignalRServices.ServiceData;

    public class GameHub : Hub
    {
        /// <summary>
        /// Service containing shared information needed by lobby and game hub.
        /// </summary>
        private readonly IHubLinkService<string, GameData, ObjectRemovedEventArgs> hubLink;

        /// <summary>
        /// Service to handle the <see cref="GameData"/> objects editing.
        /// </summary>
        private readonly IGameService gameService;

        /// <summary>
        /// Converter service for converting messages in a format that allows them to be transmitted.
        /// </summary>
        private readonly IMessageFormatConverterService<string> messageConverterService;

        /// <summary>
        /// Timer service that allows for starting of timers that run in the background.
        /// </summary>
        private readonly IBackgroundTimerService timerService;

        /// <summary>
        /// Mapping game IDs to their specified turn timer tokens. Allows for cancelling the timers created by the
        /// <see cref="IBackgroundTimerService"/> service. 
        /// </summary>
        private readonly IObjectMapService<string, CancellationTokenSource> timerMapService;

        /// <summary>
        /// Logging service for game hub.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Mapping playerNames to their specified <see cref="ISignalRClient"/> instance.
        /// </summary>
        private readonly IObjectMapService<string, SignalRGameClient> clientMapService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameHub"/> class.
        /// </summary>
        /// <param name="hubLink">The hub link.</param>
        /// <param name="gameService">The game management service.</param>
        /// <param name="messageConverterService">The message converter service.</param>
        /// <param name="timerService">The timer service.</param>
        /// <param name="timerMapService">The service to map gameIDs to <see cref="IBackgroundTimerService"/> Cancellationtokens.</param>
        /// <param name="logger">The logger of the gamehub.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if a service is null.
        /// </exception>
        public GameHub(
            IHubLinkService<string, GameData, ObjectRemovedEventArgs> hubLink,
            IGameService gameService,
            IMessageFormatConverterService<string> messageConverterService,
            IBackgroundTimerService timerService,
            IObjectMapService<string, CancellationTokenSource> timerMapService,
            ILogger<GameHub> logger,
            IObjectMapService<string, SignalRGameClient> clientMapService
            )
        {
            this.hubLink = hubLink ?? throw new ArgumentNullException(nameof(hubLink), "Hub link must not be null.");
            this.gameService = gameService ?? throw new ArgumentNullException(nameof(gameService), "Game service must not be null.");
            this.messageConverterService = messageConverterService ?? throw new ArgumentNullException(nameof(messageConverterService), "MessageConverter must not be null.");
            this.timerMapService = timerMapService ?? throw new ArgumentNullException(nameof(timerMapService), "Time mapper must not be null.");
            this.timerService = timerService ?? throw new ArgumentNullException(nameof(timerService), "Timer must not be null.");
            this.logger = logger ?? throw new ArgumentNullException(nameof(hubLink), "Logger must not be null.");
            this.clientMapService = clientMapService ?? throw new ArgumentNullException(nameof(clientMapService), "Client mapper must not be null.");
        }

        /// <summary>
        /// Handles a game move from a player.
        /// </summary>
        /// <param name="clientParameters">The parameters from the client at least consisting: gameID, playerID, playerName and column .</param>
        /// <returns></returns>
        public async Task PerformGameMove(string clientParameters)
        {
            ClientToGameHubParameters p = null;
            try
            {
                p = await this.messageConverterService.ConvertBackAsync<ClientToGameHubParameters>(clientParameters);
            }
            catch (ArgumentNullException e) 
            {
                await base.Clients.Caller.SendAsync("WrongParams");
                logger.LogWarning(e.Message);
                return;
            }
            catch (FormatConversionException e)
            {
                await base.Clients.Caller.SendAsync("WrongParams");
                logger.LogWarning(e.Message);
                return;
            }

            var invalidGameMoveJson = await this.messageConverterService.ConvertAsync(new GameHubToClientParameters()
            {
                GameID = p.GameID,
                Column = p.Column
            });

            try
            {
                logger.LogInformation($"Client: {Context.ConnectionId} with name: {p.PlayerName} requested move in game: {p.GameID}");

                var moveData = await this.gameService.MakeGameMoveAsync(p.GameID, p.PlayerID, p.PlayerName, p.Column);

                if (!moveData.IsValid)
                {
                    logger.LogInformation($"Move of client: {Context.ConnectionId} with name: {p.PlayerName} in game: {p.GameID} was not valid.");
                    await base.Clients.Caller.SendAsync("MoveInvalid", invalidGameMoveJson);
                    return;
                }

                logger.LogInformation($"Client: {Context.ConnectionId} with name: {p.PlayerName} makes move in game: {p.GameID} in column: {p.Column}.");

                // Stop timer of game.
                var turnOver = await this.timerMapService.GetValueAsync(p.GameID);
                await this.timerMapService.TryDeleteEntryAsync(p.GameID);
                if (!turnOver.IsCancellationRequested)
                {
                    turnOver.Cancel();
                }

                turnOver.Dispose();

                // Send move.
                var gameMoveJson = await this.messageConverterService.ConvertAsync(moveData);
                await base.Clients.Group(p.GameID).SendAsync("MoveDone", gameMoveJson);

                // Send winner!
                if (moveData.HasWon)
                {
                    logger.LogInformation($"Client: {Context.ConnectionId} with name: {p.PlayerName} wins game: {p.GameID}.");

                    var winnerJson = await this.messageConverterService.ConvertAsync(new GameHubToClientParameters()
                    {
                        GameID = p.GameID,
                        PlayerName = p.PlayerName,
                    });
                    await base.Clients.Group(p.GameID).SendAsync("Winner", winnerJson);

                    // Terminate the game.
                    logger.LogInformation($"Game: {p.GameID} will be deleted.");
                    await this.gameService.TerminateGameAsync(p.GameID, p.PlayerID);

                    var gameClosedJson = await this.messageConverterService.ConvertAsync(new GameHubToClientParameters() { GameID = p.GameID });
                    await base.Clients.Group(p.GameID).SendAsync("GameClosed", gameClosedJson);

                    await this.hubLink.RemoveGameAsync(p.GameID);
                    return;
                }

                // Send gameboard is full!
                if (moveData.IsGameBoardFull)
                {
                    logger.LogInformation($"Client: {Context.ConnectionId} with name: {p.PlayerName} filled gameboard of the game: {p.GameID}.");

                    await base.Clients.Group(p.GameID).SendAsync("BoardFull", p.GameID);

                    // Terminate the game.
                    logger.LogInformation($"Game: {p.GameID} will be deleted.");
                    await this.gameService.TerminateGameAsync(p.GameID, p.PlayerID);

                    var gameClosedJson = await this.messageConverterService.ConvertAsync(new GameHubToClientParameters() { GameID = p.GameID });
                    await base.Clients.Group(p.GameID).SendAsync("GameClosed", gameClosedJson);

                    await this.hubLink.RemoveGameAsync(p.GameID);

                    return;
                }

                // Next turn!
                await this.gameService.ChangeCurrentPlayerAsync(p.GameID);

                var newCurrentPlayer = await this.gameService.GetCurrentPlayer(p.GameID);
                var gameDataJson = await this.messageConverterService.ConvertAsync(new GameHubToClientParameters() { GameID = p.GameID, PlayerName = newCurrentPlayer });
                await base.Clients.Group(p.GameID).SendAsync("TurnOf", gameDataJson);

                // Start timer again!
                var gameTurnToken = new CancellationTokenSource();
                var turntime = await this.gameService.GetGameTurnTime(p.GameID);
                _ = this.timerService.StartTimerAsync(turntime, gameTurnToken.Token, HandleCurrentTurnTimerOver, p.GameID);
                await this.timerMapService.StoreEntryAsync(p.GameID, gameTurnToken);

                logger.LogInformation($"New turn in Game: {p.GameID}. CurrentPlayer: {await this.gameService.GetCurrentPlayer(p.GameID)}.");
            }
            catch (ObjectDisposedException e)
            {
                await base.Clients.Caller.SendAsync("Warning", invalidGameMoveJson);
                logger.LogError(e.Message);
                return;
            }
            catch (AggregateException e)
            {
                await base.Clients.Caller.SendAsync("Warning", invalidGameMoveJson);
                logger.LogError(e.Message);
                return;
            }
            catch (ArgumentNullException e)
            {
                await base.Clients.Caller.SendAsync("Warning", invalidGameMoveJson);
                logger.LogWarning(e.Message);
                return;
            }
            catch (InvalidOperationException e)
            {
                await base.Clients.Caller.SendAsync("Warning", invalidGameMoveJson);
                logger.LogWarning(e.Message);
                return;
            }
            catch (ArgumentException e)
            {
                await base.Clients.Caller.SendAsync("Warning", invalidGameMoveJson);
                logger.LogWarning(e.Message);
                return;
            }
        }

        /// <summary>
        /// Terminates a game.
        /// </summary>
        /// <param name="clientParameters">The parameters from the client at least consisting: gameID, playerID and playerName.</param>
        /// <returns></returns>
        public async Task CloseGame(string clientParameters)
        {
            ClientToGameHubParameters p;
            try
            {
                p = await this.messageConverterService.ConvertBackAsync<ClientToGameHubParameters>(clientParameters);

            }
            catch (ArgumentNullException e)
            {
                await base.Clients.Caller.SendAsync("WrongParams");
                logger.LogWarning(e.Message);
                return;
            }
            catch (FormatConversionException e)
            {
                await base.Clients.Caller.SendAsync("WrongParams");
                logger.LogWarning(e.Message);
                return;
            }

            try
            {
                logger.LogInformation($"Client: {p.PlayerName} wants to close the game: {p.GameID}.");

                if (!await this.clientMapService.TryGetValueAsync(p.PlayerName, out SignalRGameClient client))
                    return;

                if (!client.games.ContainsKey(p.GameID))
                {
                    var notAllowedJson = await this.messageConverterService.ConvertAsync($"You are not allowed to close game: {p.GameID}");
                    await base.Clients.Caller.SendAsync("Warning", notAllowedJson);
                    return;
                }

                var isTerminated = await this.gameService.TerminateGameAsync(p.GameID, p.PlayerID);
                if (isTerminated)
                {
                    await this.hubLink.RemoveGameAsync(p.GameID);

                    var gameTurnToken = await this.timerMapService.GetValueAsync(p.GameID);
                    gameTurnToken.Cancel();
                    gameTurnToken.Dispose();

                    await this.timerMapService.TryDeleteEntryAsync(p.GameID);

                    client.games.TryRemove(p.GameID, out _);
                    logger.LogInformation($"Client: {p.PlayerName} closed the game: {p.GameID}.");

                    var gameClosedJson = await this.messageConverterService.ConvertAsync(new GameHubToClientParameters() { GameID = p.GameID });
                    await base.Clients.Group(p.GameID).SendAsync("GameClosed", gameClosedJson);
                }
            }
            catch (ArgumentNullException e)
            {
                logger.LogWarning(e.Message);
                return;
            }
            catch (ArgumentException e)
            {
                logger.LogWarning(e.Message);
                return;
            }
            catch (ObjectDisposedException e)
            {
                logger.LogError(e.Message);
                return;
            }
        }

        /// <summary>
        /// Cuts the connection between the client and a game.
        /// </summary>
        /// <param name="clientParameters">The parameters from the client at least consisting: gameID and playerName.</param>
        /// <returns></returns>
        public async Task LeaveGame(string clientParameters)
        {
            ClientToGameHubParameters p;
            try
            {
                p = await this.messageConverterService.ConvertBackAsync<ClientToGameHubParameters>(clientParameters);

            }
            catch (ArgumentNullException e)
            {
                await base.Clients.Caller.SendAsync("WrongParams");
                logger.LogError(e.Message);
                return;
            }
            catch (FormatConversionException e)
            {
                await base.Clients.Caller.SendAsync("WrongParams");
                logger.LogError(e.Message);
                return;
            }

            try
            {
                if (!await this.clientMapService.TryGetValueAsync(p.PlayerName, out SignalRGameClient client))
                    return;

                if (!client.games.ContainsKey(p.GameID))
                {
                    return;
                }

                await base.Groups.RemoveFromGroupAsync(Context.ConnectionId, p.GameID);

                client.games.TryRemove(p.GameID, out _);
                logger.LogInformation($"Client: {p.PlayerName} left game: {p.GameID}.");
            }
            catch (ArgumentNullException e)
            {
                logger.LogWarning(e.Message);
                return;
            }
        }

        /// <summary>
        /// Handles the integration of a spectator in a game.
        /// </summary>
        /// <param name="clientParameters">The parameters from the client at least consisting: gameID and playerName.</param>
        /// <returns></returns>
        public async Task AddClientAsSpectator(string clientParameters)
        {
            ClientToGameHubParameters p;
            try
            {
                p = await this.messageConverterService.ConvertBackAsync<ClientToGameHubParameters>(clientParameters);
            }
            catch (ArgumentNullException e)
            {
                await base.Clients.Caller.SendAsync("WrongParams");
                logger.LogWarning(e.Message);
                return;
            }
            catch (FormatConversionException e)
            {
                await base.Clients.Caller.SendAsync("WrongParams");
                logger.LogWarning(e.Message);
                return;
            }

            try
            {
                var gameData = await this.gameService.GetCurrentGameStateAsync(p.GameID);

                var client = await this.clientMapService.GetValueAsync(p.PlayerName);
                client.games.TryAdd(p.GameID, p.GameID);

                await base.Groups.AddToGroupAsync(base.Context.ConnectionId, p.GameID);
                var currentGameStateJson = await this.messageConverterService.ConvertAsync(gameData);

                await base.Clients.Caller.SendAsync("IsWatching", currentGameStateJson);
            }
            catch (ArgumentNullException e)
            {
                var noWatchingJson = await this.messageConverterService.ConvertAsync($"You can not watch the game: {p.GameID}.");
                await base.Clients.Caller.SendAsync("Warning", noWatchingJson);
                logger.LogWarning(e.Message);
            }
            catch (ArgumentException e)
            {
                var noWatchingJson = await this.messageConverterService.ConvertAsync($"There is no game with ID: {p.GameID} available.");
                await base.Clients.Caller.SendAsync("Warning", noWatchingJson);
                logger.LogWarning(e.Message);
            }
        }

        /// <summary>
        /// Checks if the client is a valid player of this game and if true adds him to the game.
        /// </summary>
        /// <param name="clientParameters">The ID of the game.</param>
        /// <returns></returns>
        public async Task VerifyPlayer(string clientParameters)
        {
            ClientToGameHubParameters p;
            try
            {
                p = await this.messageConverterService.ConvertBackAsync<ClientToGameHubParameters>(clientParameters);
            }
            catch (ArgumentNullException e)
            {
                await base.Clients.Caller.SendAsync("WrongParams");
                logger.LogError(e.Message);
                return;
            }
            catch (FormatConversionException e)
            {
                await base.Clients.Caller.SendAsync("WrongParams");
                logger.LogError(e.Message);
                return;
            }

            logger.LogInformation($"Client: {p.PlayerName} wants to verify as a player for game: {p.GameID}.");

            try
            {
                if (!await this.hubLink.GameDataMap.TryGetValueAsync(p.GameID, out GameData game))
                    return;

                await this.gameService.StoreGame(p.GameID, game);

                var isNotPlayerJson = await this.messageConverterService.ConvertAsync(new GameHubToClientParameters()
                {
                    GameID = p.GameID,
                    IsPlayer = false
                });

                var isPlayer = await this.gameService.VerifyPlayerAsync(p.GameID, p.PlayerID, p.PlayerName);
                
                if (isPlayer)
                {
                    logger.LogInformation($"Client: {p.PlayerName} is a player of game: {p.GameID}.");

                    var client = await this.clientMapService.GetValueAsync(p.PlayerName);
                    client.games.TryAdd(p.GameID, p.GameID);

                    await base.Groups.AddToGroupAsync(Context.ConnectionId, p.GameID);
                    var isPlayerJson = await this.messageConverterService.ConvertAsync(new GameHubToClientParameters()
                    {
                        GameID = p.GameID,
                        IsPlayer = true
                    });
                    await base.Clients.Caller.SendAsync("IsPlayer", isPlayerJson);

                    if (await this.gameService.IsGameFull(p.GameID))
                    {
                        await this.gameService.ChangeCurrentPlayerAsync(p.GameID);

                        var currentPlayer = await this.gameService.GetCurrentPlayer(p.GameID);

                        var gameStartedJson = await this.messageConverterService.ConvertAsync(new GameHubToClientParameters()
                        {
                            GameID = p.GameID,
                            PlayerName = currentPlayer
                        });
                        await base.Clients.Group(p.GameID).SendAsync("TurnOf", gameStartedJson);

                        // Start timer
                        var gameTurnToken = new CancellationTokenSource();
                        var turntime = await this.gameService.GetGameTurnTime(p.GameID);
                        _ = this.timerService.StartTimerAsync(turntime, gameTurnToken.Token, HandleCurrentTurnTimerOver, p.GameID);
                        await this.timerMapService.StoreEntryAsync(p.GameID, gameTurnToken);
                    }
                    return;
                }

                await base.Clients.Caller.SendAsync("IsPlayer", isNotPlayerJson);
            }
            catch (ArgumentNullException e)
            {
                logger.LogWarning(e.Message);
            }
        }

        /// <summary>
        /// Notifies players of the game that the current turn is over.
        /// </summary>
        /// <param name="gameID">The ID of the game.</param>
        /// <returns></returns>
        private async Task HandleCurrentTurnTimerOver(string gameID)
        {
            try
            {
                var players = await this.gameService.GetNonCurrentPlayers(gameID);

                players.ForEach(async playerName =>
                {
                    var player = await this.clientMapService.GetValueAsync(playerName);
                    var turnOverJson = await messageConverterService.ConvertAsync(new GameHubToClientParameters() { GameID = gameID });
                    await player.ClientProxy.SendAsync("TurnOver", turnOverJson);
                });
            }
            catch (ArgumentNullException e)
            {
                logger.LogWarning(e.Message);
            }
            catch (KeyNotFoundException)
            {
                logger.LogWarning("Both players have left the game. Game will be closed.");
                await this.timerMapService.TryDeleteEntryAsync(gameID);
                await this.hubLink.RemoveGameAsync(gameID);
            }
            catch (ArgumentException e)
            {
                logger.LogWarning(e.Message);
            }
        }

        /// <summary>
        /// The player retreives the current game state if he is a player of the game.
        /// </summary>
        /// <param name="clientParameters">The parameters from the client at least consisting: gameID and playerName.</param>
        /// <returns></returns>
        public async Task ReconnectPlayer(string clientParameters)
        {
            ClientToGameHubParameters p;
            try
            {
                p = await this.messageConverterService.ConvertBackAsync<ClientToGameHubParameters>(clientParameters);

            }
            catch (ArgumentNullException e)
            {
                await base.Clients.Caller.SendAsync("WrongParams");
                logger.LogError(e.Message);
                return;
            }
            catch (FormatConversionException e)
            {
                await base.Clients.Caller.SendAsync("WrongParams");
                logger.LogError(e.Message);
                return;
            }

            try
            {
                logger.LogInformation($"Client: {Context.ConnectionId} wants to reconnect on {p.GameID}.");

                if (await this.gameService.IsPlayerOfGameAsync(p.GameID, p.PlayerName))
                {
                    var client = await this.clientMapService.GetValueAsync(p.PlayerName);
                    client.games.TryAdd(p.GameID, p.GameID);
                    
                    await base.Groups.AddToGroupAsync(Context.ConnectionId, p.GameID);
                    var gameData = this.gameService.GetCurrentGameStateAsync(p.GameID);
                    var gameParameters = this.messageConverterService.ConvertAsync(gameData);
                    await base.Clients.Caller.SendAsync("IsWatching", gameParameters);

                    var isCurrentPlayer = await this.gameService.IsPlayersTurnAsync(p.GameID, p.PlayerName);
                    var reconnectParameters = await this.messageConverterService.ConvertAsync(new GameHubToClientParameters()
                    {
                        GameID = p.GameID,
                        IsPlayer = true,
                        IsCurrentPlayer = isCurrentPlayer,
                    });
                    await base.Clients.Caller.SendAsync("ReconnectedOn", reconnectParameters);
                    return;
                }

                var notReconnectParameters = await this.messageConverterService.ConvertAsync(new GameHubToClientParameters()
                {
                    GameID = p.GameID,
                    IsPlayer = false,
                    IsCurrentPlayer = false,
                });
                await base.Clients.Caller.SendAsync("ReconnectedOn", notReconnectParameters);
            }
            catch (ArgumentNullException e)
            {
                logger.LogError(e.Message);
            }
        }

        /// <summary>
        /// Handles the connection of a user to the hub.
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            var requestedUsername = this.Context.GetHttpContext().Request.RouteValues["requestedUsername"];

            var userName = requestedUsername.ToString();
            await this.clientMapService.StoreEntryAsync(userName, new SignalRGameClient(userName, base.Clients.Caller, base.Context.ConnectionId));

            logger.LogInformation($"New client: {Context.ConnectionId} connected.");
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Handles the disconnection of a user from the hub.
        /// </summary>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var clients = await this.clientMapService.GetAllValuesAsync();

            SignalRGameClient disconnectedClient = null;
            foreach (var client in clients)
            {
                if (client.ConnectionID == Context.ConnectionId)
                {
                    disconnectedClient = client;
                }
            }

            foreach (var game in disconnectedClient.games.Keys)
            {
                await base.Groups.RemoveFromGroupAsync(disconnectedClient.ConnectionID, game);
            }

            await this.clientMapService.TryDeleteEntryAsync(disconnectedClient.ClientName);

            logger.LogInformation($"Client: {Context.ConnectionId} disconnected!");
            await base.OnDisconnectedAsync(exception);
        } 
    }
}
