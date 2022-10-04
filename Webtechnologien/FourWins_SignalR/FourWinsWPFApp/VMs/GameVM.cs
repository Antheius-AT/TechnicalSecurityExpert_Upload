//-----------------------------------------------------------------------
// <copyright file="GameVM.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Tamara Mayer</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.VMs
{
    using FourWinsWPFApp.Commands;
    using FourWinsWPFApp.Models;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Represents a game that is shown in the ingame view.
    /// </summary>
    public class GameVM : INotifyPropertyChanged
    {
        /// <summary>
        /// Is the color for the tab header.
        /// </summary>
        private SolidColorBrush headerColor;

        /// <summary>
        /// The Bitmap Source that is the Mark for player one.
        /// </summary>
        public BitmapSource PlayerOneMarkImage { get; private set; }

        /// <summary>
        /// The Bitmap Source that is the Mark for player two.
        /// </summary>
        public BitmapSource PlayerTwoMarkImage { get; private set; }

        /// <summary>
        /// A log for each individual game.
        /// </summary>
        public ObservableCollection<string> GameLog { get; private set; }

        /// <summary>
        /// Is the color for the tab header. <see cref="headerColor"/>
        /// </summary>
        public SolidColorBrush HeaderColor
        {
            get
            {
                return this.headerColor;
            }
            set
            {
                this.headerColor = value;
                this.Notify();
            }
        }

        /// <summary>
        /// Initializes a new instanze of the <see cref="GameVM"/>.
        /// </summary>
        /// <param name="gameID">The game ID.</param>
        /// <param name="playerID">The player ID, may be null, but only if isPlayer is false.</param>
        /// <param name="playerOne">The user name of player one.</param>
        /// <param name="playerTwo">The user name of player two.</param>
        /// <param name="isPlayer">True if is one of the players, false if spectator.</param>
        public GameVM(string gameID, string playerID, string playerOne, string playerTwo, bool isPlayer)
        {
            if (string.IsNullOrWhiteSpace(gameID))
                throw new ArgumentNullException(nameof(gameID), "The specified parameter must not be null!");

            if (string.IsNullOrWhiteSpace(playerOne))
                throw new ArgumentNullException(nameof(playerOne), "The specified parameter must not be null!");

            if (string.IsNullOrWhiteSpace(playerTwo))
                throw new ArgumentNullException(nameof(playerTwo), "The specified parameter must not be null!");

            if (string.IsNullOrWhiteSpace(playerID) && isPlayer)
                throw new ArgumentNullException(nameof(playerID), $"The specified parameter must not be null while {nameof(isPlayer)} is true!");

            this.Game = new Game(gameID, playerOne, playerTwo, isPlayer, playerID);

            this.GameLog = new ObservableCollection<string>();
            this.GameField = new BitmapSource[7][];

            for (int i = 0; i < this.GameField.Length; i++)
            {
                this.GameField[i] = new BitmapSource[6];
            }

            this.SetBitmapSources();
        }

        /// <summary>
        /// Sets the bitmapsources for the image marks for the players.
        /// </summary>
        private void SetBitmapSources()
        {
            using (Bitmap btm = new Bitmap(99, 99))
            {
                using (Graphics grf = Graphics.FromImage(btm))
                {
                    grf.FillEllipse(new SolidBrush(System.Drawing.Color.Blue), 0, 0, 99, 99);
                }

                this.PlayerOneMarkImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    btm.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                    );

                using (Graphics grf = Graphics.FromImage(btm))
                {
                    grf.FillEllipse(new SolidBrush(System.Drawing.Color.Red), 0, 0, 99, 99);
                }

                this.PlayerTwoMarkImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    btm.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                    );
            }

            if (this.PlayerOne.ToLower().Contains("nagel"))
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                System.IO.Stream file = assembly.GetManifestResourceStream("FourWinsWPFApp.PlayerImages.ChristianNagel.jpg");
                Bitmap btm = new Bitmap(file);

                this.PlayerOneMarkImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    btm.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                    );
            }
            else if (this.PlayerTwo.ToLower().Contains("nagel"))
            {
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                System.IO.Stream file = assembly.GetManifestResourceStream("FourWinsWPFApp.PlayerImages.ChristianNagel.jpg");
                Bitmap btm = new Bitmap(file);

                this.PlayerTwoMarkImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    btm.GetHbitmap(),
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                    );
            }
        }

        /// <summary>
        /// Notifies subscribers that some property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The visibility of the tab headers.
        /// But is somehow not working properly right now, but most likely just a mistake in the xaml
        /// </summary>
        public Visibility Visibility
        {
            get
            {
                if (this.Game.IsStarted)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Hidden;
                    //return Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// A command for leaving the game.
        /// </summary>
        public ICommand LeaveGameCommand
        {
            get
            {
                return new BasicCommand(obj =>
                {
                    this.LeaveGameRequested?.Invoke(this, EventArgs.Empty);
                });
            }
        }

        /// <summary>
        /// A command for leaving the game.
        /// </summary>
        public ICommand CloseGameCommand
        {
            get
            {
                return new BasicCommand(obj =>
                {
                    this.CloseGameRequested?.Invoke(this, EventArgs.Empty);
                });
            }
        }

        /// <summary>
        /// Notifies subscribers that a leave game is requested.
        /// </summary>
        public event EventHandler<EventArgs> LeaveGameRequested;

        /// <summary>
        /// Notifies subscribers that a close game is requested.
        /// </summary>
        public event EventHandler<EventArgs> CloseGameRequested;

        /// <summary>
        /// The ID the game has on the server.
        /// </summary>
        public string GameID
        {
            get
            {
                return this.Game.GameID;
            }
        }

        /// <summary>
        /// The ID the player have on the server.
        /// </summary>
        public string PlayerID
        {
            get
            {
                return this.Game.PlayerID;
            }
        }

        /// <summary>
        /// The username of player one.
        /// </summary>
        public string PlayerOne
        {
            get
            {
                return this.Game.PlayerOne;
            }
        }
        /// <summary>
        /// The username of player two.
        /// </summary>
        public string PlayerTwo
        {
            get
            {
                return this.Game.PlayerTwo;
            }
        }
        /// <summary>
        /// True if the client is one of the players, false if spectator.
        /// </summary>
        public bool IsPlayer
        {
            get
            {
                return this.Game.IsPlayer;
            }
        }

        /// <summary>
        /// A instance of the <see cref="Game"/>^.
        /// </summary>
        public Game Game { get; }

        /// <summary>
        /// The game Field filled with the marks of the players.
        /// </summary>
        public BitmapSource[][] GameField { get; private set; }

        /// <summary>
        /// Sets the player mark into the gamefield at the right position.
        /// </summary>
        /// <param name="player">The player that makes the move.</param>
        /// <param name="column">The cloumn in which the mark shall be placed.</param>
        public void SetPlayerMark(string player, int column)
        {
            int row = this.Game.SetMark(player, column);

            if (player == this.PlayerOne)
            {
                this.GameField[column][row] = this.PlayerOneMarkImage;
            }
            else
            {
                this.GameField[column][row] = this.PlayerTwoMarkImage;
            }

            this.Notify(nameof(GameField));
        }

        /// <summary>
        /// Rises the PropertyChanged Event.
        /// </summary>
        /// <param name="property">The name of the property.</param>
        private void Notify([CallerMemberName] string property = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
