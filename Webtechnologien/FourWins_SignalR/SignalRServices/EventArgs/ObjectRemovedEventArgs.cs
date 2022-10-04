//-----------------------------------------------------------------------
// <copyright file="ObjectRemovedEventArgs.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SignalRServices.EventArgs
{
    using System;

    /// <summary>
    /// Represent event arguments for the <see cref="IHubLinkService."/>
    /// </summary>
    public class ObjectRemovedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectRemovedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="gameID">The ID of the game that was removed..</param>
        public ObjectRemovedEventArgs(string gameID)
        {
            if (gameID == null)
                throw new ArgumentNullException(nameof(gameID), "Data must not be null.");

            this.GameID = gameID;
        }

        /// <summary>
        /// Gets the ID of the game data that was removed.
        /// </summary>
        public string GameID
        {
            get;
        }
    }
}
