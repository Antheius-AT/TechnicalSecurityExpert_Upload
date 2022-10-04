//-----------------------------------------------------------------------
// <copyright file="LobbyVM.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Tamara Mayer</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp
{
    using FourWinsWPFApp.Interfaces;
    using System.Windows;

    /// <summary>
    /// Interaktionslogik für Lobby.xaml
    /// </summary>
    public partial class Lobby : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Lobby"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public Lobby(ILobbyVM lobbyVM)
        {
            InitializeComponent();
            this.DataContext = lobbyVM;
        }
    }
}
