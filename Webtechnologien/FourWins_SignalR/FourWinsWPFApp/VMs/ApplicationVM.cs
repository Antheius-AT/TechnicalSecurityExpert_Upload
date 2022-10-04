//-----------------------------------------------------------------------
// <copyright file="ApplicationVM.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Tamara Mayer.</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.VMs
{
    using FourWinsWPFApp.EventArguments;
    using FourWinsWPFApp.Interfaces;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading;
    using System.Windows;

    public class ApplicationVM
    {
        /// <summary>
        /// The view model for the lobby.
        /// </summary>
        private ILobbyVM lobbyVM;

        /// <summary>
        /// The view model for the login.
        /// </summary>
        private ILoginVM<ErrorOccurredEventArgs> loginVM;

        /// <summary>
        /// The view model for the active games.
        /// </summary>
        private IActiveGamesVM activeGamesVM;

        /// <summary>
        /// The view for the lobby.
        /// </summary>
        private Lobby lobbyView;

        /// <summary>
        /// The view for the active games.
        /// </summary>
        private InGame activeGamesView;

        /// <summary>
        /// The view for the login.
        /// </summary>
        private Login loginView;

        /// <summary>
        /// If true the active game view is open, otherwise false.
        /// </summary>
        private bool isActiveGameViewOpen;

        /// <summary>
        /// If true the lobby view is open, otherwise false.
        /// </summary>
        private bool isLobbyViewOpen;

        /// <summary>
        /// A Thread to watch the views.
        /// </summary>
        private Thread viewWatcherThread;

        /// <summary>
        /// Represent a logger object.
        /// </summary>
        private ILogger loggerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationVM"/> class.
        /// </summary>
        /// <param name="loginVM">The view model for the login.</param>
        /// <param name="logger">The logger.</param>
        public ApplicationVM(ILoginVM<ErrorOccurredEventArgs> loginVM, ILogger<ApplicationVM> logger)
        {
            this.loggerService = logger;

            this.loginVM = loginVM;
            this.loginVM.ConnectionSuccessful_CreateWindows += LoginVM_ConnectionSuccessful;

            this.loginView = new Login(loginVM);
            this.loginView.Closed += LoginView_Closed;
            this.loginView.Show();

            this.isActiveGameViewOpen = false;
            this.isLobbyViewOpen = false;

            this.viewWatcherThread = new Thread(this.WatchViews);
            this.viewWatcherThread.IsBackground = true;
        }

        /// <summary>
        /// Reacts when the login view was closed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void LoginView_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Opens the active Game view and the lobby view after the connection in login was successful.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments containing the neccessary informtion.</param>
        private void LoginVM_ConnectionSuccessful(object sender, CreateWindowsEventArgs e)
        {
            this.activeGamesVM = e.ActiveGamesVM;
            this.lobbyVM = e.LobbyVM;

            this.viewWatcherThread = new Thread(this.WatchViews);
            this.viewWatcherThread.IsBackground = true;

            this.lobbyVM.GameCreationRequested += LobbyVM_GameCreationRequested;
            this.lobbyVM.WatchingGameRequested += LobbyVM_WatchingGameRequested;

            this.activeGamesView = new InGame(this.activeGamesVM);
            this.activeGamesView.Closed += ActiveGamesView_Closed;
            this.lobbyView = new Lobby(this.lobbyVM);
            this.lobbyView.Closed += LobbyView_Closed;

            Application.Current.Dispatcher.Invoke(() =>
            {
                this.lobbyView.Show();
                this.activeGamesView.Show();

                this.isActiveGameViewOpen = true;
                this.isLobbyViewOpen = true;

                this.loginView.Visibility = Visibility.Hidden;
            });
            this.loggerService.LogInformation("Lobby and GameView were started.");

            this.viewWatcherThread.Start();
        }

        /// <summary>
        /// Watches the views and checks regularly if they are still open, if both or the active games are closed the
        /// login opens up again. Plan was to make to connection possible again, but we did not do that.
        /// </summary>
        private void WatchViews()
        {
            while (true)
            {
                if (!this.isActiveGameViewOpen)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.lobbyView.Close();
                        this.loginView.Visibility = Visibility.Visible;

                        this.loginView.Close();
                    });

                    this.viewWatcherThread.Join();
                }

                if (!this.isActiveGameViewOpen && !this.isLobbyViewOpen)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.loginView.Visibility = Visibility.Visible;

                        this.loginView.Close();
                    });

                    this.viewWatcherThread.Join();
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Called when the lobby view is closed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event agruments.</param>
        private void LobbyView_Closed(object sender, EventArgs e)
        {
            this.isLobbyViewOpen = false;
        }

        /// <summary>
        /// Called when the active game view is closed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event agruments.</param>
        private void ActiveGamesView_Closed(object sender, EventArgs e)
        {
            this.isActiveGameViewOpen = false;
        }

        /// <summary>
        /// Calls the Method to watch a game in the active game view model.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event agruments containing the neccessary information.</param>
        private void LobbyVM_WatchingGameRequested(object sender, WatchingGameRequestedEventArgs e)
        {
            this.activeGamesVM.RequestGameWatching(e);
        }

        /// <summary>
        /// Calls the Method to create a new game in the active game view model.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event agruments containing the neccessary information.</param>
        private void LobbyVM_GameCreationRequested(object sender, GameCreationRequestedEventArgs e)
        {
            this.activeGamesVM.AddGame(e);
        }
    }
}
