//-----------------------------------------------------------------------
// <copyright file="LoginVM.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Gregor Faiman, Tamara Mayer</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.Vms
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using FourWinsWPFApp.EventArguments;
    using FourWinsWPFApp.Interfaces;
    using FourWinsWPFApp.Models;
    using Microsoft.Extensions.Logging;
    using SharedData.LobbyData;
    using SharedData.SharedHubData.Interfaces;

    /// <summary>
    /// Represents a login view model capable of handling logins.
    /// </summary>
    public class LoginVM : ILoginVM<ErrorOccurredEventArgs>
    {
        /// <summary>
        /// Represents a value indicating whether the client is connected.
        /// </summary>
        private bool connected;

        /// <summary>
        /// Represents a view model handling lobby related transactions.
        /// </summary>
        private ILobbyVM lobbyVM;

        /// <summary>
        /// Represent a view model handling transactions regarding active games.
        /// </summary>
        private IActiveGamesVM activeGameVM;

        /// <summary>
        /// Represents a factory to create SignalR clients.
        /// </summary>
        private IClientFactoryService<SignalRClient> clientFactory;

        /// <summary>
        /// Represents connection info used for connecting to a remote endpoint.
        /// </summary>
        private ConnectionInfo connectionInfo;

        /// <summary>
        /// Represent a service capable of converting data to and from a specified format.
        /// </summary>
        private IMessageFormatConverterService<string> messageConverterService;

        /// <summary>
        /// Represents a mapping service for the lobby VM.
        /// </summary>
        private IObjectMapService<string, Challenge> challengeMapService;

        /// <summary>
        /// Represents a logging service for the lobby view model.
        /// </summary>
        private ILogger<LobbyVM> lobbyLoggingService;

        /// <summary>
        /// Represents a logging service for the active game view model.
        /// </summary>
        private ILogger<ActiveGamesVM> activeGameVMLoggingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginVM"/> class.
        /// </summary>
        /// <param name="loginService">The service used for logging in.</param>
        public LoginVM(IClientFactoryService<SignalRClient> loginService, IMessageFormatConverterService<string> messageConverterService, IObjectMapService<string, Challenge> challengeMapService, ILogger<LobbyVM> lobbyVMLoggingService,ILogger<ActiveGamesVM> activeGameVMLoggingService)
        {
            this.clientFactory = loginService;
            this.connectionInfo = new ConnectionInfo();
            this.messageConverterService = messageConverterService;
            this.connected = false;
            this.challengeMapService = challengeMapService;
            this.lobbyLoggingService = lobbyVMLoggingService;
            this.activeGameVMLoggingService = activeGameVMLoggingService;
        }

        /// <summary>
        /// Notifies subscribing views that closing is requested.
        /// </summary>
        public event EventHandler ClosingRequested;

        /// <summary>
        /// Notifies subscribers that a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies subscribers that some error occurred.
        /// </summary>
        public event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

        /// <summary>
        /// Notifies subscribers that the connections are successfull and the windows shall be created.
        /// </summary>
        public event EventHandler<CreateWindowsEventArgs> ConnectionSuccessful_CreateWindows;

        /// <summary>
        /// Gets or sets the lobby view model.
        /// </summary>
        public ILobbyVM LobbyViewModel
        {
            get
            {
                return this.lobbyVM;
            }

            set
            {
                this.lobbyVM = value ?? throw new ArgumentNullException(nameof(value), "Lobby vm must not be null.");
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the active games view model.
        /// </summary>
        public IActiveGamesVM ActiveGamesVM
        {
            get
            {
                return this.activeGameVM;
            }

            set
            {
                this.activeGameVM = value ?? throw new ArgumentNullException(nameof(value), "Active game vm must not be null.");
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if username is null.
        /// </exception>
        public string Username
        {
            get
            {
                return this.connectionInfo.Username;
            }

            set
            {
                this.connectionInfo.Username = value ?? throw new ArgumentNullException(nameof(value), "Username must not be null.");
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the server URL.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if you attempt to set null.
        /// </exception>
        /// <remarks>This is needed because I do not want the view to bind directly
        /// to the model. Thus, I created two properties that the view can reference, which internally
        /// reference properties in the model, which serves as clear separation between data and view.</remarks>
        public Uri ServerURL
        {
            get
            {
                return this.connectionInfo.ServerURL;
            }

            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "Uri must not be null.");

                // TODO: Set exception validation on textbox in view to show error.
                if (!Uri.TryCreate(value.ToString(), UriKind.Absolute, out Uri parsedValue))
                    throw new ArgumentException(nameof(value), "Value was not a valid uri.");

                this.connectionInfo.ServerURL = parsedValue;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the command for logging in.
        /// </summary>
        public ICommand LoginCommand
        {
            get
            {
                return new RelayCommand(async _ =>
                {
                    await Task.Run(async () =>
                    {
                        SignalRClient lobbyClient;
                        bool success;

                        this.connected = true;

                        lobbyClient = this.clientFactory.CreateClient(this.connectionInfo, $"fwLobby/{connectionInfo.Username}");

                        this.LobbyViewModel = new LobbyVM(lobbyClient, this.messageConverterService, this.challengeMapService, this.lobbyLoggingService);

                        this.LobbyViewModel.InvalidUsernameConnectionFailed += this.HandleLoginError;
                        this.LobbyViewModel.ConnectionClosed += this.HandleConnectionClosed;

                        success = await lobbyClient.TryConnectAsync();

                        if (!success)
                        {
                            this.connected = false;
                            this.RaiseErrorOccurred("Could not establish connection as serverURL was invalid.");
                            this.RaisePropertyChanged();
                        }
                        else
                        {
                            //TODO Wait to ensure username is valid and connection is not terminated.
                            //Not sure if this is needed, requires testing.
                            //Update: It is needed, however need to play around with timings.
                             await Task.Delay(2000);

                            if (this.connected)
                            {
                                //TODO maybe I need the dispatcher for it too, but i don't think so
                                //var activeGameVM= new ActiveGamesVM(client, this.messageConverterService,connectionInfo.Username);

                                //TODO this is "old" code, can be deleted here soon xD
                                await Application.Current.Dispatcher.Invoke(async () =>
                                {
                                    SignalRClient gameClient;

                                    // hi
                                    //TODO noch schöner hinbekommen, das is hochgradig hingewixxt. Funktioniert außerdem noch nicht
                                    gameClient = this.clientFactory.CreateClient(this.connectionInfo, $"fwGame/{connectionInfo.Username}");
                                    var success = await gameClient.TryConnectAsync();

                                    this.ActiveGamesVM = new ActiveGamesVM(gameClient, this.messageConverterService, gameClient.ClientName,this.activeGameVMLoggingService);

                                    var lobbyView = new Lobby(this.LobbyViewModel);
                                    var activeGameView = new InGame(this.ActiveGamesVM);

                                    this.RaiseConnectionSuccessful_CreateWidows(this.ActiveGamesVM, this.LobbyViewModel);

                                    //lobbyView.Show();
                                    //activeGameView.Show();

                                    //TODO eigentlich ist der plan das login view offen zu lassen, sollte funktionieren wenn VMs
                                    //gespeichert sind, wird zu einem späteren zeitpunkt noch eingebaut
                                    //this.RaiseClosingRequested();
                                });
                            }
                            else
                            {
                                this.connected = false;
                                this.RaisePropertyChanged();
                            }
                        }
                    });
                },
                p => !this.connected);
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyPath">The changed property.</param>
        protected virtual void RaisePropertyChanged([CallerMemberName]string propertyPath = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyPath));
        }

        /// <summary>
        /// Raises the <see cref="ClosingRequested"/> event.
        /// </summary>
        protected virtual void RaiseClosingRequested()
        {
            this.ClosingRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="ErrorOccurred"/> event.
        /// </summary>
        /// <param name="message">The error message associated with the occurred error.</param>
        protected virtual void RaiseErrorOccurred(string message)
        {
            this.ErrorOccurred?.Invoke(this, new ErrorOccurredEventArgs(message));
        }

        /// <summary>
        /// Raises the <see cref="ConnectionSuccessful_CreateWindows"/> event.
        /// </summary>
        protected virtual void RaiseConnectionSuccessful_CreateWidows(IActiveGamesVM activeGamesVM, ILobbyVM lobbyVM)
        {
            this.ConnectionSuccessful_CreateWindows?.Invoke(this, new CreateWindowsEventArgs(activeGamesVM,lobbyVM));
        }

        /// <summary>
        /// Handles the event in which the hub terminates the established connection.
        /// </summary>
        /// <param name="arg">The paramter sent from the hub.</param>
        /// <returns>A task object handling the callback.</returns>
        private void HandleConnectionClosed(object source, ConnectionClosedEventArguments e)
        {
            this.connected = false;
        }

        /// <summary>
        /// Handles login errors due to an invalid username.
        /// </summary>
        /// <param name="serverResponse">The server response.</param>
        private void HandleLoginError(object source, InvalidUsernameEventArguments e)
        {
            this.RaiseErrorOccurred(e.ErrorMessage);
        }
    }
}
