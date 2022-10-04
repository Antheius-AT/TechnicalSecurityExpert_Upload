//-----------------------------------------------------------------------
// <copyright file="CreateWindowsEventArgs.cs" company="FHWN">
//     Copyright (c) FHWN. All rights reserved.
// </copyright>
// <author>Tamara Mayer</author>
//-----------------------------------------------------------------------
namespace FourWinsWPFApp.Interfaces
{
    public class CreateWindowsEventArgs
    {
        public IActiveGamesVM ActiveGamesVM { get; private set; }
        public ILobbyVM LobbyVM { get; private set; }

        public CreateWindowsEventArgs(IActiveGamesVM activeGamesVM, ILobbyVM lobbyVM)
        {
            this.ActiveGamesVM = activeGamesVM;
            this.LobbyVM = lobbyVM;
        }
    }
}