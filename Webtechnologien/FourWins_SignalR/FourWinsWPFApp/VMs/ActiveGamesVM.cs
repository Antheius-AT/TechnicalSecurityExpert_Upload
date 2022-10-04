//-----------------------------------------------------------------------
// <copyright file="ActiveGamesVM.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Tamara Mayer.</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.Vms
{
    using FourWinsWPFApp.EventArguments;
    using FourWinsWPFApp.Interfaces;
    using FourWinsWPFApp.Models;
    using FourWinsWPFApp.VMs;
    using SharedData.SharedHubData.Interfaces;
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR.Client;
    using FourWins_SignalRServer.HubData;
    using System.Collections.Generic;
    using System.Linq;
    using SignalRServices.ServiceData;
    using SharedData.Exceptions;
    using System.Windows;
    using SharedData.GameData;
    using Microsoft.Extensions.Logging;

    public class ActiveGamesVM : IActiveGamesVM
    {
        /// <summary>
        /// A list of all games that are currently open on the client.
        /// </summary>
        public ObservableCollection<GameVM> AllGames { get; private set; }

        /// <summary>
        /// Gets or sets the client used by this view model.
        /// </summary>
        public IClientContract Client
        {
            get;
        }

        /// <summary>
        /// Represents a logging service for the active game view model.
        /// </summary>
        private ILogger<ActiveGamesVM> loggerService;

        /// <summary>
        /// The userName this client is logged on to the server with.
        /// </summary>
        private string myUserName;

        /// <summary>
        /// A locker for the list of games.
        /// </summary>
        private object gameLocker;

        /// <summary>
        /// Service to convert messages to and from a specified format.
        /// </summary>
        private IMessageFormatConverterService<string> messageConverterService;

        /// <summary>
        /// Notifies subscribers that the connection was closed.
        /// </summary>
        public event EventHandler<ConnectionClosedEventArguments> ConnectionClosed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveGamesVM"/> class.
        /// </summary>
        /// <param name="client">The signalR client for communicating with the server.</param>
        /// <param name="messageConverterService">The message converter service for converting and reconverting messages.</param>
        /// <param name="userName">The username with which this client is currently logged in into the game.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either of the parameters are null.
        /// </exception>
        public ActiveGamesVM(SignalRClient client, IMessageFormatConverterService<string> messageConverterService, string userName, Microsoft.Extensions.Logging.ILogger<ActiveGamesVM> activeGameVMLoggingService)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client), "Client must not be null.");

            if (messageConverterService == null)
                throw new ArgumentNullException(nameof(messageConverterService), "Message converter service must not be null.");

            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentNullException(nameof(userName), "User Name must not be null nor whitespace nor empty.");

            client.HubConnection.Closed += HandleConnectionClosed;
            client.HubConnection.Reconnected += HandleReconnectionSuccessful;
            client.HubConnection.On<string>("MoveDone", this.MakeMove);
            client.HubConnection.On<string>("TurnOf", this.SetTurnOf);
            client.HubConnection.On<string>("IsPlayer", this.StartGame);
            client.HubConnection.On<string>("IsWatching", this.StartWatching);
            client.HubConnection.On<string>("MoveInvalid", this.MoveInvalid);
            client.HubConnection.On<string>("TurnOver", this.TurnOver);
            client.HubConnection.On<string>("GameClosed", this.GameClosed);
            client.HubConnection.On<string>("Winner", this.GameWon);
            client.HubConnection.On<string>("BoardFull", this.BoardFull);
            client.HubConnection.On<string>("ReconnectedOn", this.ReconnectedToGame);

            this.Client = client;

            this.loggerService = activeGameVMLoggingService;
            this.messageConverterService = messageConverterService;
            this.myUserName = userName;
            this.AllGames = new ObservableCollection<GameVM>();
            this.gameLocker = new object();
        }

        /// <summary>
        /// Reacts to the board full method from the game hub.
        /// </summary>
        /// <param name="response">The message from the gameHub that contains the neccessary information.</param>
        /// <returns>A task object handling the callback.</returns>
        private async Task BoardFull(string response)
        {
            GameHubToClientParameters parsedResponse;
            IEnumerable<GameVM> games;

            try
            {
                parsedResponse = await this.messageConverterService.ConvertBackAsync<GameHubToClientParameters>(response);
            }
            catch (ArgumentNullException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(BoardFull)} was null");
                return;
            }
            catch (FormatConversionException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(BoardFull)} was in the wrong format.");
                return;
            }

            lock (gameLocker)
            {
                games = AllGames.Where(g => g.GameID == parsedResponse.GameID);

                if (games.Count() != 1)
                {
                    return;
                }

                games.ElementAt(0).GameLog.Add("Game board is full.");
            }
        }

        /// <summary>
        /// Reacts to the reconnected on method from the game hub.
        /// </summary>
        /// <param name="response">The message from the gameHub that contains the neccessary information.</param>
        /// <returns>A task object handling the callback.</returns>
        private async Task ReconnectedToGame(string response)
        {
            GameHubToClientParameters parsedResponse;
            IEnumerable<GameVM> games;

            try
            {
                parsedResponse = await this.messageConverterService.ConvertBackAsync<GameHubToClientParameters>(response);
            }
            catch (ArgumentNullException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(GameWon)} was null");
                return;
            }
            catch (FormatConversionException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(GameWon)} was in the wrong format.");
                return;
            }

            if (!parsedResponse.IsCurrentPlayer && !parsedResponse.IsPlayer)
                return;

            lock (gameLocker)
            {
                games = AllGames.Where(g => g.GameID == parsedResponse.GameID);

                if (games.Count() != 1)
                {
                    return;
                }

                games.ElementAt(0).GameLog.Add($"{parsedResponse.PlayerName} has won the game!");
            }
        }

        /// <summary>
        /// Reacts to the game won method from the game hub.
        /// </summary>
        /// <param name="response">The message from the gameHub that contains the neccessary information.</param>
        /// <returns>A task object handling the callback.</returns>
        private async Task GameWon(string response)
        {
            GameHubToClientParameters parsedResponse;
            IEnumerable<GameVM> games;

            try
            {
                parsedResponse = await this.messageConverterService.ConvertBackAsync<GameHubToClientParameters>(response);
            }
            catch (ArgumentNullException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(GameWon)} was null");
                return;
            }
            catch (FormatConversionException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(GameWon)} was in the wrong format.");
                return;
            }

            lock (gameLocker)
            {
                games = AllGames.Where(g => g.GameID == parsedResponse.GameID);

                if (games.Count() != 1)
                {
                    return;
                }

                games.ElementAt(0).GameLog.Add($"{parsedResponse.PlayerName} has won the game!");
            }
        }

        /// <summary>
        /// Reacts to the game closed method from the game hub.
        /// </summary>
        /// <param name="response">The message from the gameHub that contains the neccessary information.</param>
        /// <returns>A task object handling the callback.</returns>
        private async Task GameClosed(string response)
        {
            GameHubToClientParameters parsedResponse;
            IEnumerable<GameVM> games;

            try
            {
                parsedResponse = await this.messageConverterService.ConvertBackAsync<GameHubToClientParameters>(response);
            }
            catch (ArgumentNullException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(GameClosed)} was null");
                return;
            }
            catch (FormatConversionException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(GameClosed)} was in the wrong format.");
                return;
            }

            lock (gameLocker)
            {
                games = AllGames.Where(g => g.GameID == parsedResponse.GameID);

                if (games.Count() != 1)
                {
                    return;
                }

                games.ElementAt(0).Game.IsClosed = true;
                games.ElementAt(0).GameLog.Add("The game was closed.");
            }
        }

        /// <summary>
        /// Reacts to the turn over method from the game hub.
        /// </summary>
        /// <param name="response">The message from the gameHub that contains the neccessary information.</param>
        /// <returns>A task object handling the callback.</returns>
        private async Task TurnOver(string response)
        {
            GameHubToClientParameters parsedResponse;
            IEnumerable<GameVM> games;

            try
            {
                parsedResponse = await this.messageConverterService.ConvertBackAsync<GameHubToClientParameters>(response);
            }
            catch (ArgumentNullException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(TurnOver)} was null");
                return;
            }
            catch (FormatConversionException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(TurnOver)} was in the wrong format.");
                return;
            }

            lock (gameLocker)
            {
                games = AllGames.Where(g => g.GameID == parsedResponse.GameID);

                if (games.Count() != 1)
                {
                    return;
                }

                games.ElementAt(0).GameLog.Add("The others player time for the turn is over.");
                games.ElementAt(0).HeaderColor = ActiveGameVMColor.NotifyColor_TabHeader;
            }
        }

        /// <summary>
        /// Reacts the the move invalid method from the game hub.
        /// </summary>
        /// <param name="response">The message from the gameHub that contains the neccessary information.</param>
        /// <returns>A task object handling the callback.</returns>
        private async Task MoveInvalid(string response)
        {
            GameHubToClientParameters parsedResponse;
            IEnumerable<GameVM> games;

            try
            {
                parsedResponse = await this.messageConverterService.ConvertBackAsync<GameHubToClientParameters>(response);
            }
            catch (ArgumentNullException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(MoveInvalid)} was null");
                return;
            }
            catch (FormatConversionException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(MoveInvalid)} was in the wrong format.");
                return;
            }

            lock (gameLocker)
            {
                games = AllGames.Where(g => g.GameID == parsedResponse.GameID);

                if (games.Count() != 1)
                {
                    return;
                }

                games.ElementAt(0).GameLog.Add($"Placing a mark inside column {parsedResponse.Column + 1} was not possible!");
                games.ElementAt(0).GameLog.Add($"Try again.");
            }
        }

        /// <summary>
        /// Creates the game for watching and does the game moves.
        /// </summary>
        /// <param name="response">The message from the gameHub that contains the neccessary information.</param>
        /// <returns>A task object handling the callback.</returns>
        private async Task StartWatching(string response)
        {
            GameStateData parsedResponse;
            GameVM newGame;

            try
            {
                parsedResponse = await this.messageConverterService.ConvertBackAsync<GameStateData>(response);
            }
            catch (ArgumentNullException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(StartWatching)} was null");
                return;
            }
            catch (FormatConversionException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(StartWatching)} was in the wrong format.");
                return;
            }

            newGame = new GameVM(parsedResponse.GameID, null, parsedResponse.Players[0], parsedResponse.Players[1], false);

            for (int i = 0; i < parsedResponse.ValidGameMovesDone.Count; i++)
            {
                newGame.SetPlayerMark(parsedResponse.ValidGameMovesDone.ElementAt(i).PlayerName, parsedResponse.ValidGameMovesDone.ElementAt(i).Column);
            }

            lock (this.gameLocker)
            {
                this.AllGames.Add(newGame);
            }
        }

        /// <summary>
        /// Starts the game officially.
        /// </summary>
        /// <param name="response">The message from the gameHub that contains the neccessary information.</param>
        /// <returns>A task object handling the callback.</returns>
        private async Task StartGame(string response)
        {
            GameHubToClientParameters parsedResponse;
            IEnumerable<GameVM> games;

            try
            {
                parsedResponse = await this.messageConverterService.ConvertBackAsync<GameHubToClientParameters>(response);
            }
            catch (ArgumentNullException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(StartGame)} was null");
                return;
            }
            catch (FormatConversionException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(StartGame)} was in the wrong format.");
                return;
            }

            lock (gameLocker)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    games = AllGames.Where(g => g.GameID == parsedResponse.GameID);

                    if (games.Count() != 1)
                    {
                        return;
                    }

                    games.ElementAt(0).Game.IsStarted = true;
                    games.ElementAt(0).HeaderColor = ActiveGameVMColor.NotifyColor_TabHeader;
                });
            }
        }

        /// <summary>
        /// Sets the name of the player whos turn it is.
        /// </summary>
        /// <param name="response">The message from the gameHub that contains the neccessary information.</param>
        /// <returns>A task object handling the callback.</returns>
        private async Task SetTurnOf(string response)
        {
            GameHubToClientParameters parsedResponse;
            IEnumerable<GameVM> games;

            try
            {
                parsedResponse = await this.messageConverterService.ConvertBackAsync<GameHubToClientParameters>(response);
            }
            catch (ArgumentNullException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(SetTurnOf)} was null");
                return;
            }
            catch (FormatConversionException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(SetTurnOf)} was in the wrong format.");
                return;
            }

            lock (gameLocker)
            {
                games = AllGames.Where(g => g.GameID == parsedResponse.GameID);

                if (games.Count() != 1)
                {
                    return;
                }

                games.ElementAt(0).Game.TurnOf_UserName = parsedResponse.PlayerName;

                if (games.ElementAt(0).Game.IsPlayer && this.myUserName == parsedResponse.PlayerName)
                {
                    games.ElementAt(0).GameLog.Add("It is your turn.");
                    //TODO hier weil es was wichtiges ist?!
                    games.ElementAt(0).HeaderColor = ActiveGameVMColor.NotifyColor_TabHeader;
                }
                else
                {
                    games.ElementAt(0).GameLog.Add($"Turn of {parsedResponse.PlayerName}.");
                }
                //TODO oder soll einfach generll a notify kommen weil was passiert ist?
                //Oder noch mehr differentiaten between specator and player?
            }
        }

        /// <summary>
        /// Makes a game move accoring to the information in the response.
        /// </summary>
        /// <param name="response">The message from the gameHub that contains the neccessary information.</param>
        /// <returns>A task object handling the logic.</returns>
        private async Task MakeMove(string response)
        {
            GameMoveData parsedResponse;
            IEnumerable<GameVM> games;

            try
            {
                parsedResponse = await this.messageConverterService.ConvertBackAsync<GameMoveData>(response);
            }
            catch (ArgumentNullException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(MakeMove)} was null");
                return;
            }
            catch (FormatConversionException)
            {
                this.loggerService.LogInformation($"The respone in the {nameof(MakeMove)} was in the wrong format.");
                return;
            }

            lock (gameLocker)
            {
                games = AllGames.Where(g => g.GameID == parsedResponse.GameID);

                if (games.Count() != 1)
                {
                    return;
                }

                games.ElementAt(0).SetPlayerMark(parsedResponse.CurrentPlayer, parsedResponse.Column);
            }
        }

        /// <summary>
        /// Sends the move to be made to the GameHub.
        /// </summary>
        /// <param name="gameID">The id of the game.</param>
        /// <param name="playerID">The id of the players.</param>
        /// <param name="column">The column in which the mark shall be placed.</param>
        /// <returns>A task object handling the logic.</returns>
        public async Task SendMoveToServer(string gameID, string playerID, int column)
        {
            lock (gameLocker)
            {
                IEnumerable<GameVM> games = AllGames.Where(g => g.GameID == gameID);

                if (games.Count() != 1)
                {
                    return;
                }

                if (!games.ElementAt(0).Game.IsPlayer)
                {
                    return;
                }

                if (games.ElementAt(0).Game.TurnOf_UserName != myUserName)
                {
                    return;
                }
            }

            var formattedRequest = await this.messageConverterService.ConvertAsync(new ClientToGameHubParameters()
            {
                GameID = gameID,
                PlayerID = playerID,
                PlayerName = this.myUserName,
                Column = column
            });

            await this.Client.SendMessageAsync("PerformGameMove", formattedRequest);
        }

        /// <summary>
        /// Handles the event in which the connection between client and server is terminated.
        /// </summary>
        /// <param name="e">The exception, containing information about whether the connection
        /// was terminated due to an error or due to a client request.</param>
        /// <returns>A task object handling the logic.</returns>
        private Task HandleConnectionClosed(Exception e)
        {
            //TODO don't know if i like this personally, have to talk about this/see how it is while testing
            if (e == null)
                this.RaiseConnectionClosed("Connection was terminated as requestd by the client.");
            else
                this.RaiseConnectionClosed("Connection was terminated due to a server error.", e);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the event in which a reconnection to the server was successful.
        /// </summary>
        /// <param name="response">The server response.</param>
        /// <returns>A task object handling the logic.</returns>
        private Task HandleReconnectionSuccessful(string response)
        {
            //TODO Dont know when this gets called, need to experiment!
            throw new NotImplementedException();
        }

        /// <summary>
        /// Raises the <see cref="ConnectionClosed"/> event.
        /// </summary>
        /// <param name="message">The informational message.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if message is null.
        /// </exception>
        protected virtual void RaiseConnectionClosed(string message, Exception e = null)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message), "Message must not be null.");

            this.ConnectionClosed?.Invoke(this, new ConnectionClosedEventArguments(message));
        }

        /// <summary>
        /// Adds a new Game to the List.
        /// </summary>
        /// <param name="e">The event arguments with the information for the game.</param>
        /// <returns>A task object handling the callback.</returns>
        public async Task AddGame(GameCreationRequestedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GameVM game = new GameVM(e.GameID, e.PlayerID, e.PlayerOne, e.PlayerTwo, true);
                game.LeaveGameRequested += this.LeaveGame;
                game.CloseGameRequested += this.Game_CloseGameRequested;

                lock (gameLocker)
                {
                    this.AllGames.Add(game);
                }
            });

            var request = new ClientToGameHubParameters()
            {
                GameID = e.GameID,
                PlayerID = e.PlayerID,
                PlayerName = myUserName
            };

            var formattedRequest = await this.messageConverterService.ConvertAsync(request);

            await this.Client.SendMessageAsync("VerifyPlayer", formattedRequest);
        }

        /// <summary>
        /// Sends the close game method to the game Hub.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void Game_CloseGameRequested(object sender, EventArgs e)
        {
            GameVM game = sender as GameVM;

            var request = new ClientToGameHubParameters()
            {
                GameID = game.GameID,
                PlayerID = game.PlayerID,
                PlayerName = this.myUserName
            };

            var formattedRequest = await this.messageConverterService.ConvertAsync(request);

            await this.Client.SendMessageAsync("CloseGame", formattedRequest);

            lock (gameLocker)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AllGames.Remove(game);
                });
            }
        }

        /// <summary>
        /// Requests the information for a game to watch.
        /// </summary>
        /// <param name="e">The event arguments with the information for the game.</param>
        /// <returns>A task object handling the callback.</returns>
        public async Task RequestGameWatching(WatchingGameRequestedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e), $"The {typeof(WatchingGameRequestedEventArgs)} must not be null!");


            var request = new ClientToGameHubParameters()
            {
                GameID = e.GameID,
                PlayerName = this.myUserName
            };

            var formattedRequest = await this.messageConverterService.ConvertAsync(request);

            await this.Client.SendMessageAsync("AddClientAsSpectator", formattedRequest);
        }

        /// <summary>
        /// Sends the leave game method to the game Hub.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        public async void LeaveGame(object sender, EventArgs e)
        {
            GameVM game = sender as GameVM;

            var request = new ClientToGameHubParameters()
            {
                GameID = game.GameID,
                PlayerName = this.myUserName
            };

            var formattedRequest = await this.messageConverterService.ConvertAsync(request);

            await this.Client.SendMessageAsync("LeaveGame", formattedRequest);

            lock (gameLocker)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AllGames.Remove(game);
                });
            }
        }
    }
}
