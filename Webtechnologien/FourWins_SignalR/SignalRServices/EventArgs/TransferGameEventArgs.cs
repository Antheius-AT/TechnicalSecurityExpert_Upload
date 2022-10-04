//-----------------------------------------------------------------------
// <copyright file="TransferGameEventArgs.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SignalRServices.EventArgs
{
    using System;
    using SignalRServices.Interfaces;
    using SignalRServices.ServiceData;

    /// <summary>
    /// Event argument for the <see cref="IHubLinkService.GameCreated"/> event.
    /// </summary>
    public class TransferGameEventArgs : EventArgs
    {
        public readonly CreatedGameData gameData;
        public readonly string[] players;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferGameEventArgs"/> class.
        /// </summary>
        /// <param name="gameID">The game id.</param>
        /// <param name="players">The array of players.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if players array is null.
        /// </exception>
        public TransferGameEventArgs(CreatedGameData gameData, string[] players)
        {
            if (players == null)
                throw new ArgumentNullException(nameof(players), "Players must not be null.");

            if (gameData == null)
                throw new ArgumentNullException(nameof(gameData), "Game ID must not be null.");

            this.gameData = gameData;
            this.players = players;
        }

    }
}
