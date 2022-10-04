//-----------------------------------------------------------------------
// <copyright file="LobbyVM.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.Vms
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using FourWinsWPFApp.EventArguments;
    using FourWinsWPFApp.Interfaces;
    using FourWinsWPFApp.Models;
    using Microsoft.AspNetCore.SignalR.Client;
    using Microsoft.Extensions.Logging;
    using SharedData.Exceptions;
    using SharedData.LobbyData;
    using SharedData.LobbyData.Requests;
    using SharedData.LobbyData.ResponseMessages;
    using SharedData.SharedHubData.Interfaces;

    /// <summary>
    /// Represents the view model that lobby views can bind to.
    /// </summary>
    public class LobbyVM : ILobbyVM
    {
        /// <summary>
        /// Lock object to synchronize requests to the <see cref="Players"/> collection.
        /// </summary>
        private object playersListLock;

        /// <summary>
        /// Lock object to synchronize requests to the <see cref="ActiveGames"/> collection.
        /// </summary>
        private object gameListLock;

        /// <summary>
        /// Observable collection storing currently active games.
        /// </summary>
        private ObservableCollection<WatchableGame> activeGames;

        /// <summary>
        /// Observable collection storing currently active players.
        /// </summary>
        private ObservableCollection<ChallengeablePlayerViewModel> players;

        /// <summary>
        /// Service to convert messages to and from a specified format.
        /// </summary>
        private IMessageFormatConverterService<string> messageConverterService;

        /// <summary>
        /// A service that maps challenge objects to keys.
        /// </summary>
        private IObjectMapService<string, Challenge> challengeMapService;

        /// <summary>
        /// Represent a logger object.
        /// </summary>
        private ILogger<LobbyVM> loggerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LobbyVM"/> class.
        /// </summary>
        /// <param name="client">The signalR client for communicating with the server.</param>
        /// <param name="messageConverterService">The message converter service for converting and reconverting messages.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either of the parameters are null.
        /// </exception>
        public LobbyVM(SignalRClient client, IMessageFormatConverterService<string> messageConverterService, IObjectMapService<string, Challenge> challengeMapService, ILogger<LobbyVM> loggerService)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client), "Client must not be null.");

            if (messageConverterService == null)
                throw new ArgumentNullException(nameof(messageConverterService), "Message converter service must not be null.");

            client.HubConnection.Closed += HandleConnectionClosed;
            client.HubConnection.Reconnected += HandleReconnectionSuccessful;
            client.HubConnection.On<string>("InitializeClientList", this.InitializeClientList);
            client.HubConnection.On<string>("InitializeGameList", this.InitializeGameList);
            client.HubConnection.On<string>("UpdateClientList", this.UpdateClientList);
            client.HubConnection.On<string>("UpdateGameList", this.UpdateGameList);
            client.HubConnection.On<string>("LoginError", this.HandleLoginError);
            client.HubConnection.On<string>("ChallengeResponse", this.HandleChallengeResponse);
            client.HubConnection.On<string>("MatchCreated", this.HandleGameCreated);
            client.HubConnection.On<string>("ForwardChallenge", this.HandleChallengeReceived);
            client.HubConnection.On<string>("ForwardChallengeError", this.HandleChallengeIssueError);
            client.HubConnection.On<string>("ChallengeTimeout", this.HandleChallengeTimeout);

            this.Client = client;

            this.messageConverterService = messageConverterService;
            this.challengeMapService = challengeMapService;
            this.loggerService = loggerService;

            this.ActiveGames = new ObservableCollection<WatchableGame>();
            this.Players = new ObservableCollection<ChallengeablePlayerViewModel>();

            this.playersListLock = new object();
            this.gameListLock = new object();
        }

        /// <summary>
        /// Notifies subscribers that some property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies subscribers that the connection was closed.
        /// </summary>
        public event EventHandler<ConnectionClosedEventArguments> ConnectionClosed;

        /// <summary>
        /// Notifies subscribers that a connection failed due to an invalid username.
        /// </summary>
        public event EventHandler<InvalidUsernameEventArguments> InvalidUsernameConnectionFailed;

        /// <summary>
        /// Notifies subscribers that a new game is to be created.
        /// </summary>
        public event EventHandler<GameCreationRequestedEventArgs> GameCreationRequested;

        /// <summary>
        /// Notifies subscribers that the client requested to watch a game.
        /// </summary>
        public event EventHandler<WatchingGameRequestedEventArgs> WatchingGameRequested;

        /// <summary>
        /// Gets or sets the client used by this view model.
        /// </summary>
        public IClientContract Client
        {
            get;
        }

        /// <summary>
        /// Gets the command to challenge another player.
        /// </summary>
        public ICommand ChallengePlayerCommand
        {
            get
            {
                return new RelayCommand(async challenged =>
                {
                    var challengedPlayerVM = challenged as ChallengeablePlayerViewModel;

                    if (challengedPlayerVM == null)
                        throw new ArgumentException(nameof(challenged), $"Command parameter must be an instance of {typeof(ChallengeablePlayer)}");

                    challengedPlayerVM.ModifyPlayerChallengeStatus(ChallengeStatus.ChallengeOutgoing);

                    var request = new ChallengeRequest(this.Client.ClientName, challengedPlayerVM.Player.Username, $"{this.Client.ClientName} challenged {challengedPlayerVM.Player.Username} to a match");
                    var formattedRequest = await this.messageConverterService.ConvertAsync(request);

                    await this.Client.SendMessageAsync("DelegateChallengeAsync", formattedRequest);
                },
                challenged =>
                {
                    var challengedPlayer = challenged as ChallengeablePlayerViewModel;

                    if (challenged != null)
                    {
                        bool isExecuteAvailable = challengedPlayer.Player.Username != this.Client.ClientName && challengedPlayer.Player.ChallengeStatus == ChallengeStatus.Available;

                        return isExecuteAvailable;
                    }

                    return false;
                });
            }
        }

        /// <summary>
        /// Gets the command that allows an invoker to watch a game.
        /// </summary>
        public ICommand WatchGameCommand
        {
            get
            {
                return new RelayCommand(gameToWatch =>
                {
                    var game = gameToWatch as WatchableGame;

                    if (game != null)
                        this.RaiseWatchingGameRequested(game.GameID);
                },
                gameToWatch => gameToWatch != null);
            }
        }

        /// <summary>
        /// Gets a collection of connected players that the client can challenge.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if you attempt to set null.
        /// </exception>
        public ObservableCollection<ChallengeablePlayerViewModel> Players
        {
            get
            {
                return this.players;
            }

            private set
            {
                this.players = value ?? throw new ArgumentNullException(nameof(value), "Collection of Players must not be null.");
            }
        }

        /// <summary>
        /// Gets or sets the collection of currently active games.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if you attempt to set null.
        /// </exception>
        public ObservableCollection<WatchableGame> ActiveGames
        {
            get
            {
                return this.activeGames;
            }

            set
            {
                this.activeGames = value ?? throw new ArgumentNullException(nameof(value), "Collections of Games must not be null.");
            }
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
        /// Raises the <see cref="InvalidUsernameConnectionFailed"/> event.
        /// </summary>
        /// <param name="message">The informational message.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if message is null.
        /// </exception>
        protected virtual void RaiseUsernameInvalid(string message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message), "Message must not be null.");

            this.InvalidUsernameConnectionFailed?.Invoke(this, new InvalidUsernameEventArguments(message));
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyPath">The changed property.</param>
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyPath = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyPath));
        }

        /// <summary>
        /// Raises the <see cref="GameCreationRequested"/> event.
        /// </summary>
        /// <param name="response">The server response containing the game data.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if response is null.
        /// </exception>
        protected virtual void RaiseGameCreationRequested(NewGameCreatedResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response), "Response must not be null.");

            this.loggerService.LogInformation($"A new game has been created for clients {response.PlayerOneUsername} and {response.PlayerTwoUsername}");
            this.GameCreationRequested?.Invoke(this, new GameCreationRequestedEventArgs(response.PlayerID, response.GameID, response.PlayerOneUsername, response.PlayerTwoUsername));
        }

        /// <summary>
        /// Raises the <see cref="WatchingGameRequested"/> event.
        /// </summary>
        /// <param name="GameID">The Id of the game that is requested to be watched.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if GameID is null.
        /// </exception>
        protected virtual void RaiseWatchingGameRequested(string GameID)
        {
            if (GameID == null)
                throw new ArgumentNullException(nameof(GameID), "Response must not be null.");

            this.WatchingGameRequested?.Invoke(this, new WatchingGameRequestedEventArgs(GameID));
        }

        /// <summary>
        /// Handles the response which contains an updated version of the client list.
        /// </summary>
        /// <param name="response">The server response.</param>
        private async Task UpdateClientList(string response)
        {
            var isConnected = await this.messageConverterService.CanConvertInto<PlayerConnectedResponse>(response);
            var isDisconnected = await this.messageConverterService.CanConvertInto<PlayerDisconnectedResponse>(response);

            if (isConnected)
            {
                var parsedResponse = await this.messageConverterService.ConvertBackAsync<PlayerConnectedResponse>(response);

                if (this.Client.ClientName != parsedResponse.PlayerUsername)
                {
                    var playerVM = new ChallengeablePlayerViewModel(new ChallengeablePlayer(parsedResponse.PlayerUsername));
                    playerVM.ChallengeAccepted += HandleChallengeAccepted;

                    lock (this.playersListLock)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            this.Players.Add(playerVM);
                        });
                    }
                }
            }
            else if (isDisconnected)
            {
                var parsedResponse = await this.messageConverterService.ConvertBackAsync<PlayerDisconnectedResponse>(response);
                if (this.Client.ClientName != parsedResponse.DisconnectedPlayerUsername)
                {
                    var playerVM = this.players.FirstOrDefault(player => player.Player.Username == parsedResponse.DisconnectedPlayerUsername);

                    if (playerVM == null)
                        throw new ArgumentException("player is null.");

                    playerVM.ChallengeAccepted -= HandleChallengeAccepted;

                    lock (this.playersListLock)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            this.Players.Remove(playerVM);
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Handles the response which contains an updated version of the game list.
        /// </summary>
        /// <param name="response">The server sent response.</param>
        /// <returns>A task object handling the logic.</returns>
        private async Task UpdateGameList(string response)
        {
            var isGameAdded = await this.messageConverterService.CanConvertInto<NewGameCreatedResponse>(response);
            var isGameDeleted = await this.messageConverterService.CanConvertInto<GameRemovedResponse>(response);

            if (isGameAdded)
            {
                NewGameCreatedResponse parsedResponse;
                WatchableGame game;

                parsedResponse = await this.messageConverterService.ConvertBackAsync<NewGameCreatedResponse>(response);

                game = new WatchableGame(parsedResponse.GameID, parsedResponse.PlayerOneUsername, parsedResponse.PlayerTwoUsername);

                if (this.ActiveGames.FirstOrDefault(item => item.GameID == game.GameID) == null)
                {
                    lock (this.gameListLock)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            this.activeGames.Add(game);
                        });
                    }
                }
            }
            else if (isGameDeleted)
            {
                GameRemovedResponse parsedResponse;

                parsedResponse = await this.messageConverterService.ConvertBackAsync<GameRemovedResponse>(response);

                var gameToRemove = this.ActiveGames.FirstOrDefault(p => p.GameID == parsedResponse.RemovedGameID);

                if (gameToRemove != null)
                {
                    lock (this.gameListLock)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            this.activeGames.Remove(gameToRemove);
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Handles the server response which initializes the list of clients and games
        /// on initially connecting.
        /// </summary>
        /// <param name="response">The server response.</param>
        private async Task InitializeClientList(string response)
        {
            InitializePlayerListResponse parsedResponse;

            parsedResponse = await this.messageConverterService.ConvertBackAsync<InitializePlayerListResponse>(response);

            foreach (var item in parsedResponse.PlayerUsernames)
            {
                if (this.Client.ClientName == item)
                    continue;

                var playerVM = new ChallengeablePlayerViewModel(new ChallengeablePlayer(item));
                playerVM.ChallengeAccepted += HandleChallengeAccepted;

                lock (this.playersListLock)
                {
                    this.Players.Add(playerVM);
                }
            }
        }

        /// <summary>
        /// Handles the server response which initializes the game list.
        /// </summary>
        /// <param name="response">The server response.</param>
        /// <returns>A task handling the logic.</returns>
        private async Task InitializeGameList(string response)
        {
            InitializeGameListResponse parsedResponse;

            parsedResponse = await this.messageConverterService.ConvertBackAsync<InitializeGameListResponse>(response);

            foreach (var item in parsedResponse.Games)
            {
                lock (this.gameListLock)
                {
                    this.ActiveGames.Add(item);
                }
            }
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
        /// Handles the event in which the connection between client and server is terminated.
        /// </summary>
        /// <param name="e">The exception, containing information about whether the connection
        /// was terminated due to an error or due to a client request.</param>
        /// <returns>A task object handling the logic.</returns>
        private Task HandleConnectionClosed(Exception e)
        {
            if (e == null)
                this.RaiseConnectionClosed("Connection was terminated as requestd by the client.");
            else
                this.RaiseConnectionClosed("Connection was terminated due to a server error.", e);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles the event in which a login error occurred.
        /// </summary>
        /// <param name="response">The server response.</param>
        /// <returns>A task object handling the login error response.</returns>
        private async Task HandleLoginError(string response)
        {
            var parsedResponse = await this.messageConverterService.ConvertBackAsync<ErrorResponse>(response);
            this.RaiseUsernameInvalid(parsedResponse.Message);
        }

        /// <summary>
        /// Handles a response message issued by the server as part of a challenge reply.
        /// </summary>
        /// <param name="response">The server response.</param>
        /// <returns>A task object handling the logic of handling the response.</returns>
        private async Task HandleChallengeResponse(string response)
        {
            // TODO: Think about how to best handle this. It is purely informational though.
            // Can either be challenge accepted or new game created.
            // I created a method in the json service which might be used here, but its
            // not the best choice.
            // Entweder Challenge Accepted, oder ChallengeDenied

            var isAccepted = await this.messageConverterService.CanConvertInto<ChallengeAcceptedResponse>(response);
            var isDenied = await this.messageConverterService.CanConvertInto<ChallengeDeniedResponse>(response);

            if (isAccepted)
            {
                var challengeAcceptedResponse = await this.messageConverterService.ConvertBackAsync<ChallengeAcceptedResponse>(response);

                if (challengeAcceptedResponse.Challenge.ChallengingPlayerUsername == this.Client.ClientName)
                {
                    var otherPlayer = this.Players.FirstOrDefault(p => p.Player.Username == challengeAcceptedResponse.Challenge.ChallengedPlayerUsername);
                    otherPlayer?.ModifyPlayerChallengeStatus(ChallengeStatus.CreatingMatch);
                    this.loggerService.LogInformation("A challenge was accepted and a game is about to be created.");
                }
            }
            else if (isDenied)
            {
                var challengeDeniedResponse = await this.messageConverterService.ConvertBackAsync<ChallengeDeniedResponse>(response);

                var otherPlayer = this.Players.FirstOrDefault(p => p.Player.Username == challengeDeniedResponse.Challenge.ChallengedPlayerUsername);
                otherPlayer?.ModifyPlayerChallengeStatus(ChallengeStatus.Available);
                this.loggerService.LogInformation("A challenge was denied");
            }
        }

        /// <summary>
        /// Handles a response message issued by the server concerning the creation of a new game.
        /// </summary>
        /// <param name="response">The response message.</param>
        /// <returns>A task object handling the logic.</returns>
        private async Task HandleGameCreated(string response)
        {
            // TODO: think about using try catch.

            NewGameCreatedResponse createdResponse;

            createdResponse = await this.messageConverterService.ConvertBackAsync<NewGameCreatedResponse>(response);

            var first = this.Players.FirstOrDefault(p => p.Player.Username == createdResponse.PlayerOneUsername);
            var second = this.Players.FirstOrDefault(p => p.Player.Username == createdResponse.PlayerTwoUsername);


            first?.ModifyPlayerChallengeStatus(ChallengeStatus.Available);
            second?.ModifyPlayerChallengeStatus(ChallengeStatus.Available);
            this.RaiseGameCreationRequested(createdResponse);
            this.loggerService.LogInformation("Game was successfully created.");

        }

        /// <summary>
        /// Handles a response message issued by the server concerning a challange issue error.
        /// </summary>
        /// <param name="response">The response message.</param>
        /// <returns>A task object handling the logic.</returns>
        private void HandleChallengeIssueError(string response)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Handles the event in which another player challenged the client.
        /// </summary>
        /// <param name="response">The server response containing the challenge sent by the other player.</param>
        /// <returns>A task object handling the logic.</returns>
        private async Task HandleChallengeReceived(string response)
        {
            try
            {
                var parsedResponse = await this.messageConverterService.ConvertBackAsync<IssuedChallengeResponse>(response);

                await this.challengeMapService.StoreEntryAsync(parsedResponse.Challenge.ChallengingPlayerUsername, parsedResponse.Challenge);

                lock (this.playersListLock)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var player = this.Players.FirstOrDefault(p => p.Player.Username == parsedResponse.Challenge.ChallengingPlayerUsername);
                        player?.ModifyPlayerChallengeStatus(ChallengeStatus.ChallengeIncoming);
                    });
                }
            }
            catch (FormatConversionException e)
            {
                this.loggerService.LogError(e.Message);
                throw;
            }
            catch (ArgumentException e)
            {
                this.loggerService.LogError(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Handles the server response that is sent if a challenge timed out.
        /// </summary>
        /// <param name="response">The server response.</param>
        /// <returns>A task object handling the logic.</returns>
        private async Task HandleChallengeTimeout(string response)
        {
            // TODO: Think about implementing try catch for converting.
            ChallengeablePlayerViewModel otherPlayer;
            var parsedResponse = await this.messageConverterService.ConvertBackAsync<ChallengeTimedOutResponse>(response);
            bool isClientChallengeIssuer = parsedResponse.Challenge.ChallengingPlayerUsername == this.Client.ClientName;
            bool isClientChallengeReceiver = parsedResponse.Challenge.ChallengedPlayerUsername == this.Client.ClientName;

            lock (this.playersListLock)
            {
                if (isClientChallengeIssuer)
                    otherPlayer = this.Players.FirstOrDefault(p => p.Player.Username == parsedResponse.Challenge.ChallengedPlayerUsername);
                else if (isClientChallengeReceiver)
                    otherPlayer = this.Players.FirstOrDefault(p => p.Player.Username == parsedResponse.Challenge.ChallengingPlayerUsername);
                else
                    throw new ArgumentException(nameof(response), "Response probably was not meant for the client, as clients username didnt show up in challenge");

                if (otherPlayer != null)
                {
                    _ = this.challengeMapService.TryDeleteEntryAsync(otherPlayer.Player.Username).Result;
                    otherPlayer.ModifyPlayerChallengeStatus(ChallengeStatus.Available);
                }
                else
                {
                    this.loggerService.LogInformation("Other player has disconnected.");
                }
            }
        }

        /// <summary>
        /// Handles the event sent from a challengeable player view model, that a challenge was accepted.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private async void HandleChallengeAccepted(object sender, ChallengeAcceptedEventArgs e)
        {
            var success = await this.challengeMapService.TryGetValueAsync(e.PlayerVM.Player.Username, out Challenge storedChallenge);

            if (!success)
                throw new InvalidOperationException("Challenge could not be accepted. It may have timed out already.");

            storedChallenge.ChallengeAccepted = true;
            var response = new IssuedChallengeResponse(storedChallenge, $"{this.Client.ClientName} accepted challenge of opponent {e.PlayerVM.Player.Username}.");
            var formattedResponse = await this.messageConverterService.ConvertAsync(response);
            await this.Client.SendMessageAsync("ForwardChallengeResponse", formattedResponse);

            e.PlayerVM.ModifyPlayerChallengeStatus(ChallengeStatus.CreatingMatch);
        }
    }
}
