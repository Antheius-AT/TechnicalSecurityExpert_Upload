//-----------------------------------------------------------------------
// <copyright file="LobbyHub.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace FourWins_SignalRServer.Hubs
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using FourWins_SignalRServer.ClientContract;
    using Microsoft.Extensions.Logging;
    using SharedData.LobbyData;
    using SharedData.LobbyData.ResponseMessages;
    using SignalRServices.Interfaces.ServiceInterfaces.SharedServices;
    using SignalRServices.Interfaces.ServiceInterfaces.LobbyServices;
    using SharedData.LobbyData.Interfaces;
    using SharedData.Exceptions;
    using SharedData.SharedHubData.Interfaces;
    using SignalRServices.QueueItems;
    using SignalRServices.ServiceData;
    using SignalRServices.EventArgs;
    using SignalRServices.Interfaces.DataInterfaces;
    using System.Linq;
    using SharedData.LobbyData.Requests;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// This class represents the lobby hub, which takes care of all network traffic
    /// that is associated with the lobby, like forwarding challenges, and updating list of available
    /// players.
    /// </summary>
    public class LobbyHub : Hub
    {
        /// <summary>
        /// Object mapping challenge objects to cancellation tokens. Allows for cancelling the timers created by the
        /// <see cref="IBackgroundTimerService"/> service as well as to validate whether an accepted challenge 
        /// responded to in <see cref="ForwardChallengeResponse(string)"/> is still valid.
        /// </summary>
        private readonly IObjectMapService<Challenge, CancellationTokenSource> challengeMapService;

        /// <summary>
        /// Service containing shared information needed by lobby and game hub.
        /// </summary>
        private readonly IHubLinkService<string, GameData, ObjectRemovedEventArgs> hubLinkService;

        /// <summary>
        /// Cancellation token for the background queue that pushes notifications to clients
        /// in regular intervals.
        /// </summary>
        private static CancellationTokenSource backgroundQueueCancellationToken;
        
        /// <summary>
        /// Logging service for lobby hub.
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// User map, mapping an instance of <see cref="ISignalRClient"/> to each username.
        /// </summary>
        private readonly IObjectMapService<string, ISignalRClient> clientMapService;

        /// <summary>
        /// Background queue service pushing updates to clients in regular time intervals.
        /// </summary>
        private readonly IBackgroundQueueService queueService;

        /// <summary>
        /// Converter service for converting messages in a format that allows them to be transmitted.
        /// </summary>
        private readonly IMessageFormatConverterService<string> messageFormatConverter;

        /// <summary>
        /// Timer service that allows for starting of timers that run in the background.
        /// </summary>
        private readonly IBackgroundTimerService timerService;

        public LobbyHub(ILogger<LobbyHub> logger, IHubLinkService<string, GameData, ObjectRemovedEventArgs> hubLink, IObjectMapService<string, ISignalRClient> userMap, IObjectMapService<Challenge, CancellationTokenSource> cancellationTokenMapService, IBackgroundQueueService queueService, IMessageFormatConverterService<string> messageConverterService, IBackgroundTimerService timerService)
        {
            this.hubLinkService = hubLink;
            this.clientMapService = userMap;
            this.queueService = queueService;
            this.queueService.QueueInvocationCallback = this.HandleQueueItems;
            this.logger = logger;
            this.messageFormatConverter = messageConverterService;
            this.timerService = timerService;
            this.challengeMapService = cancellationTokenMapService;
            this.hubLinkService.ObjectRemoved += HandleGameDataRemoved;

            if (!this.queueService.IsRunning)
                this.queueService.StartAsync(backgroundQueueCancellationToken.Token);
        }

        /// <summary>
        /// Initializes static members of the <see cref="LobbyHub"/> class.
        /// </summary>
        static LobbyHub()
        {
            backgroundQueueCancellationToken = new CancellationTokenSource();
        }

        /// <summary>
        /// Handles the event in which a user connects to the hub.
        /// It checks whether the specified user is valid (= not already in use).
        /// If it is not valid the hub terminates the connection and sends an error message to the client.
        /// </summary>
        /// <returns>A task object.</returns>
        public override async Task OnConnectedAsync()
        {
            // Muss mir noch überlegen wie ich die GAmes mitschicke. AM besten eigenes ActiveGameResponse Objekt
            // Wo drinnen stehn die beiden Spieler und die Game ID, weil ich die Game ID bedenkenlos rausgeben kann.
            // Die aktuellen games bekomme ich aus dem Hublink.GameDataMap.
            var requestedUsername = this.Context.GetHttpContext().Request.RouteValues["requestedUsername"];

            var errorResponse = await this.ValidateIncomingUsername(requestedUsername);

            if (!string.IsNullOrWhiteSpace(errorResponse.Message))
            {
                var formattedResponse = await this.messageFormatConverter.ConvertAsync(errorResponse);
                await this.Clients.Caller.SendAsync("LoginError", formattedResponse);
                this.Context.Abort();
                this.logger.LogError("Client tried connecting, but with invalid username.", this.Clients.Caller);
            }
            else
            {
                // First create new Signal R client and store it in the client map.
                // Then send player list to newly connected clients, and create an object of type 
                // client connected queue item, to update all the other clients that someone connected.
                var username = requestedUsername.ToString();
                var client = new SignalRClient(username, this.Clients.Caller, this.Context.ConnectionId);

                await this.clientMapService.StoreEntryAsync(username, client);

                // Send initialize player list
                var initializePlayerListResponse = await this.BuildInitializePlayerListResponse(client.ClientName);
                var formattedData = await this.messageFormatConverter.ConvertAsync(initializePlayerListResponse);
                await client.SendMessageAsync("InitializeClientList", formattedData);

                // Send initialized game list.
                var initializeGameListResponse = await this.BuildInitializeGameListResponse();
                formattedData = await this.messageFormatConverter.ConvertAsync(initializeGameListResponse);
                await client.SendMessageAsync("InitializeGameList", formattedData);

                // Enqueue an item that contains information that a client connected, destined for all the other players
                // except the connected one.
                var playerConnectedResponse = new PlayerConnectedResponse("A new player connected.", client.ClientName);
                var compositeClient = new SignalRCompositeClient(await this.clientMapService.GetAllValuesExceptAsync(client.ClientName));
                var clientConnectedItem = new ClientConnectedQueueItem(compositeClient, "UpdateClientList", playerConnectedResponse);
                await this.queueService.EnqueueAsync(clientConnectedItem);

                this.logger.LogInformation($"Client connected successfully with username {client.ClientName}.");
            }

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Handles a clients disconnection.
        /// </summary>
        /// <param name="exception">The exception parameter.</param>
        /// <returns>A task handling the disconnection logic.</returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var allClients = await this.clientMapService.GetAllValuesAsync();
            var disconnected = allClients.FirstOrDefault(p => p.ConnectionID == this.Context.ConnectionId);

            if (disconnected != null)
            {
                await this.clientMapService.TryDeleteEntryAsync(disconnected.ClientName);
            }

            var playerDisconnectedResponse = new PlayerDisconnectedResponse("A new player disconnected.", disconnected.ClientName);
            var compositeClient = new SignalRCompositeClient(await this.clientMapService.GetAllValuesExceptAsync(disconnected.ClientName));
            var clientConnectedItem = new ClientDisconnectedQueueItem(compositeClient, "UpdateClientList", playerDisconnectedResponse);
            await this.queueService.EnqueueAsync(clientConnectedItem);

            this.logger.LogInformation("A Client disconnected.");

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Delegates a challenge to the challenged client.
        /// </summary>
        /// <param name="challengingPlayerName">The username of the player that is issuing the challenge.</param>
        /// <param name="challengedPlayerName">The username of the player that is being challenged.</param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either of the parameters are null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Is thrown if the player who is challenging is the same as the player who is challenged.
        /// </exception>
        public async Task DelegateChallengeAsync(string formattedRequest)
        {
            if (formattedRequest == null)
                throw new ArgumentNullException(nameof(formattedRequest), "Request must not be null.");

            var responseSuccessCallback = "ForwardChallenge";
            var responseFailureCallback = "ForwardChallengeError";
            IResponse responseMessage;
            ChallengeRequest request;
            string formattedResponse;
            Challenge challenge;

            try
            {
                request = await this.messageFormatConverter.ConvertBackAsync<ChallengeRequest>(formattedRequest);

                if (request.ChallengeReceiver == request.ChallengeIssuer)
                    throw new InvalidOperationException("You can not issue challenges to yourself.");

                var challengedPlayer = await this.clientMapService.GetValueAsync(request.ChallengeReceiver);
                var tokenSource = new CancellationTokenSource();

                challenge = this.CreateChallenge(request.ChallengeIssuer, request.ChallengeReceiver);
                responseMessage = new IssuedChallengeResponse(challenge, $"Player {request.ChallengeIssuer} challenges you ({request.ChallengeReceiver}) to a game of four wins.");
                formattedResponse = await this.messageFormatConverter.ConvertAsync(responseMessage);

                await challengedPlayer.ClientProxy.SendAsync(responseSuccessCallback, formattedResponse);
                await this.challengeMapService.StoreEntryAsync(challenge, tokenSource);
                _ = this.timerService.StartTimerAsync(30, tokenSource.Token, this.HandleChallengeTimedOut, challenge);

                this.logger.LogInformation("Delegated challenge and started 30 second timer.");
            }
            catch (ArgumentException e)
            {
                responseMessage = new ErrorResponse("Challenge could not be forwarded to opponent. Please ensure he is still connected.");
                formattedResponse = await this.messageFormatConverter.ConvertAsync(responseMessage);
                await this.Clients.Caller.SendAsync(responseFailureCallback, formattedResponse);
                this.logger.LogError(e.Message);
            }
            catch (FormatConversionException e)
            {
                responseMessage = new ErrorResponse($"Formatted data was not a valid object of type {typeof(ChallengeRequest)}.");
                formattedResponse = await this.messageFormatConverter.ConvertAsync(responseMessage);
                await this.Clients.Caller.SendAsync(responseFailureCallback, formattedResponse);
                this.logger.LogError(e.Message);
            }
            catch (InvalidOperationException e)
            {
                responseMessage = new ErrorResponse(e.Message);
                formattedResponse = await this.messageFormatConverter.ConvertAsync(responseMessage);
                await this.Clients.Caller.SendAsync(responseFailureCallback, formattedResponse);
                this.logger.LogError(e.Message);
            }
            catch (Exception e)
            {
                responseMessage = new ErrorResponse("An error occurred during forwarding the challenge to your opponent.");
                formattedResponse = await this.messageFormatConverter.ConvertAsync(responseMessage);
                await this.Clients.Caller.SendAsync(responseFailureCallback, formattedResponse);
                this.logger.LogError(e.Message);
            }
        }

        /// <summary>
        /// Forwards the challenge response to the player who issued the challenge.
        /// </summary>
        /// <param name="clientResponse">The response sent from the client.</param>
        /// <returns>A task object responsible for handling the response.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if client response is null.
        /// </exception>
        public async Task ForwardChallengeResponse(string clientResponse)
        {
            if (clientResponse == null)
                throw new ArgumentNullException(nameof(clientResponse), "Client response must not be null.");

            var challengeResponseMethod = "ChallengeResponse";

            try
            {
                string formattedResponse;
                IResponse response;

                // Convert JSON into concrete type I can work with.
                var challengeResponse = await this.messageFormatConverter.ConvertBackAsync<IssuedChallengeResponse>(clientResponse);

                if (!await this.challengeMapService.TryGetValueAsync(challengeResponse.Challenge, out CancellationTokenSource tokenSource))
                    throw new InvalidOperationException("The challenge challenge response could not be forwarded as the challenge was already invalidated.");

                // Remove challenge from map and end timer.
                tokenSource.Cancel();
                await this.challengeMapService.TryDeleteEntryAsync(challengeResponse.Challenge);

                // Foward the challenge response.
                ISignalRClient challengeIssuer = await this.clientMapService.GetValueAsync(challengeResponse.Challenge.ChallengingPlayerUsername);

                if (challengeResponse.Challenge.ChallengeAccepted)
                    response = new ChallengeAcceptedResponse(challengeResponse.Challenge, $"{challengeResponse.Challenge.ChallengedPlayerUsername} accepted your challenge with the following challenge ID: {challengeResponse.Challenge.ChallengeID}");
                else
                    response = new ChallengeDeniedResponse(challengeResponse.Challenge, $"Your challenge regarding \"{challengeResponse.Challenge.ChallengedPlayerUsername}\" was not accepted. Challenge id: {challengeResponse.Challenge.ChallengeID}");

                formattedResponse = await this.messageFormatConverter.ConvertAsync(response);
                await challengeIssuer.ClientProxy.SendAsync(challengeResponseMethod, formattedResponse);

                if (challengeResponse.Challenge.ChallengeAccepted)
                    await this.HandleResponsibilityTransfer(challengeResponse.Challenge.ChallengingPlayerUsername, challengeResponse.Challenge.ChallengedPlayerUsername);

                this.logger.LogInformation("Delegated challenge response.");
            }
            catch (ArgumentException e)
            {
                var message = new ErrorResponse("The message could not be forwarded, as the player who initially issued the challenge could not be found");
                var formattedMessage = await this.messageFormatConverter.ConvertAsync(message);
                await this.Clients.Caller.SendAsync("ErrorResponse", formattedMessage);
                this.logger.LogError(e.Message);
            }
            catch (InvalidOperationException e)
            {
                var message = e.Message;
                var formattedMessage = await this.messageFormatConverter.ConvertAsync(message);
                await this.Clients.Caller.SendAsync("ErrorResponse", formattedMessage);
                this.logger.LogError(e.Message);
            }
            catch (FormatConversionException e)
            {
                var message = new ErrorResponse($"The sent data was received, could however not be parsed into the proper type that is required by this method. Ensure that you send a serialized object of type {typeof(IssuedChallengeResponse)} when calling this method.");
                var formattedMessage = await this.messageFormatConverter.ConvertAsync(message);
                await this.Clients.Caller.SendAsync("ErrorResponse", formattedMessage);
                this.logger.LogError(e.Message);
            }
        }

        /// <summary>
        /// Checks whether an incoming username is valid.
        /// </summary>
        /// <param name="queryString">The querystring containing the username.</param>
        /// <returns>An error message. If the error message is empty, the username is valid.</returns>
        private async Task<ErrorResponse> ValidateIncomingUsername(object queryString)
        {
            string errorMessage = string.Empty;

            if (queryString == null)
            {
                errorMessage = await this.messageFormatConverter.ConvertAsync("You need to specify a prefered user name when connecting to the lobby hub.");
                this.logger.LogError("Tried to validate username, but username was null.");
            }

            string username = queryString.ToString();

            if (await this.clientMapService.DoesEntryExistAsync(username))
            {
                errorMessage = await this.messageFormatConverter.ConvertAsync("The specified user name is already in use by another client.");
                this.logger.LogInformation($"Client tried to connect with username {username}, but it was already in use.");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = await this.messageFormatConverter.ConvertAsync("The specified user name was invalid due to only consisting of white space characters or being empty.");
                this.logger.LogInformation("Client tried to connect with invalid username.");
            }

            return new ErrorResponse(errorMessage);
        }

        /// <summary>
        /// Handles the case in which an issued challenge goes unanswered for a set period of time
        /// after which it is regarded as timed out and is no longer valid.
        /// </summary>
        /// <param name="expired">The expired challenge.</param>
        /// <returns>A task handling the logic of timing out an expired challenge.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if expired is null.
        /// </exception>
        private async Task HandleChallengeTimedOut(Challenge expired)
        {
            if (expired == null)
                throw new ArgumentNullException(nameof(expired), "Expired challenge must not be null.");

            string responseMethod = "ChallengeTimeout";

            if (await this.challengeMapService.DoesEntryExistAsync(expired))
            {
                ISignalRClient challengedClient;
                ISignalRClient challengingClient;

                await this.challengeMapService.TryDeleteEntryAsync(expired);

                await this.clientMapService.TryGetValueAsync(expired.ChallengedPlayerUsername, out challengedClient);
                await this.clientMapService.TryGetValueAsync(expired.ChallengingPlayerUsername, out challengingClient);

                var message = new ChallengeTimedOutResponse(expired, $"The challenge with id {expired.ChallengeID} expired due to {expired.ChallengedPlayerUsername} not responding on time.");
                var formattedMessage = await this.messageFormatConverter.ConvertAsync(message);

                if (challengedClient != null)
                    await challengedClient.ClientProxy.SendAsync(responseMethod, formattedMessage);
                if (challengingClient != null)
                    await challengingClient.ClientProxy.SendAsync(responseMethod, formattedMessage);
            }

            this.logger.LogInformation($"Challenge with challenge ID {expired.ChallengeID} expired.");
        }

        /// <summary>
        /// Creates an object of type <see cref="Challenge"/> which contains data related to game challenges.
        /// </summary>
        /// <param name="challengingClientUsername">The username of the client who is issuing the challenge.</param>
        /// <param name="challengedClientUsername">The username of the client that is being challenged.</param>
        /// <returns>A task object handling the logic of creating a challenge.</returns>
        private Challenge CreateChallenge(string challengingClientUsername, string challengedClientUsername)
        {
            var challengeID = Guid.NewGuid();
            var challenge = new Challenge(challengeID, challengingClientUsername, challengedClientUsername);

            this.logger.LogInformation($"Challenge with ID {challengeID} was created.");

            return challenge;
        }

        /// <summary>
        /// Builds a response message of type <see cref="InitializePlayerListResponse"/>.
        /// </summary>
        /// <returns>A task containing an initialize player list response in its result.</returns>
        private async Task<InitializePlayerListResponse> BuildInitializePlayerListResponse(string ignoredUsername)
        {
            var players = await this.clientMapService.GetAllValuesExceptAsync(ignoredUsername);
            var usernames = new List<string>();

            foreach (var item in players)
            {
                usernames.Add(item.ClientName);
            }

            return new InitializePlayerListResponse("Updated players list", usernames);
        }

        /// <summary>
        /// Builds a response message of type <see cref="InitializeGameListResponse"/>.
        /// </summary>
        /// <returns>A task containing an initialize game list response in its result.</returns>
        private async Task<InitializeGameListResponse> BuildInitializeGameListResponse()
        {
            var activeGames = await this.hubLinkService.GameDataMap.GetAllValuesAsync();
            var watchableGames = new List<WatchableGame>();

            foreach (var item in activeGames)
            {
                var gameID = await this.hubLinkService.GameDataMap.GetAssociatedKeyAsync(item);
                watchableGames.Add(new WatchableGame(gameID, item.Players.First(), item.Players.Last()));
            }

            return new InitializeGameListResponse("Initialized game list", watchableGames);
        }

        /// <summary>
        /// Handles the transition between the lobby and game hubs. 
        /// Initiates the creation of a new game, and then passes control over to the game hub
        /// for the two respective clients who agreed to play a game.
        /// </summary>
        /// <returns>A task object handling the transition logic.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if challenge issuer or challenge receiver are null.
        /// </exception>
        private async Task HandleResponsibilityTransfer(string challengeIssuer, string challengeReceiver)
        {
            ISignalRClient issuer;
            ISignalRClient receiver;
            IResponse response;
            string methodName;
            string formattedResponse;

            var issuerFound = await this.clientMapService.TryGetValueAsync(challengeIssuer, out issuer);
            var receiverFound = await this.clientMapService.TryGetValueAsync(challengeReceiver, out receiver);

            var gameData = await this.hubLinkService.CreateNewGameAsync(90);

            if (!(issuerFound && receiverFound))
            {
                methodName = "ErrorResponse";
                response = new ErrorResponse("Your opponent disconnected after accepting the challenge.");
                await this.hubLinkService.GameDataMap.TryDeleteEntryAsync(gameData.GameID);
                this.logger.LogInformation("Tried to notify opponent that challenge was accepted, however opponent disconnected.");
            }
            else
            {
                methodName = "MatchCreated";
                response = new NewGameCreatedResponse("A new game was created. Connect to game hub using your game ID and player ID", gameData.GameID, gameData.PlayerID, challengeIssuer, challengeReceiver);

                var queueItem = new GameCreatedQueueItem(new SignalRCompositeClient(await this.clientMapService.GetAllValuesAsync()), "UpdateGameList", (NewGameCreatedResponse)response);
                await this.queueService.EnqueueAsync(queueItem);
                this.logger.LogInformation("Created a match and notified players.");
            }

            formattedResponse = await this.messageFormatConverter.ConvertAsync(response);
            await issuer.ClientProxy.SendAsync(methodName, formattedResponse);
            await receiver.ClientProxy.SendAsync(methodName, formattedResponse);
        }

        /// <summary>
        /// Handles the queue items and send them to the server.
        /// </summary>
        /// <param name="queueItems">The queue items to handle.</param>
        /// <returns>A task object handling the logic.</returns>
        private async Task HandleQueueItems(IEnumerable<IQueueItem> queueItems)
        {
            foreach (var item in queueItems)
            {
                var formattedResponse = await this.messageFormatConverter.ConvertAsync(item.Response);
                await item.Receiver.SendMessageAsync(item.MethodName, formattedResponse);

                this.logger.LogInformation($"Emptied queue and sent objects to client. Amount of items sent: {queueItems.Count()}");
            }
        }

        /// <summary>
        /// Handles the event in which a game is removed from the list of monitored games.
        /// </summary>
        /// <param name="source">The event source.</param>
        /// <param name="e">The event arguments.</param>
        private async void HandleGameDataRemoved(object source, ObjectRemovedEventArgs e)
        {
            var response = new GameRemovedResponse($"The game with the game ID {e.GameID} has been removed", e.GameID);
            var queueItem = new GameRemovedQueueItem(new SignalRCompositeClient(await this.clientMapService.GetAllValuesAsync()), "UpdateGameList", response);
            await this.queueService.EnqueueAsync(queueItem);

            this.logger.LogInformation("Updated clients that game was removed.");
        }
    }
}
