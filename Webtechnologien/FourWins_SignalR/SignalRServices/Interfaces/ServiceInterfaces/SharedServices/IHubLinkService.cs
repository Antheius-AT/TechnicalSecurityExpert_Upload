//-----------------------------------------------------------------------
// <copyright file="IHubLink.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SignalRServices.Interfaces.ServiceInterfaces.SharedServices
{
    using System;
    using System.Threading.Tasks;
    using SharedData.SharedHubData.Interfaces;
    using SignalRServices.ServiceData;

    /// <summary>
    /// Interface definition for a link between game and lobby hubs.
    /// </summary>
    public interface IHubLinkService<TMapKey, TMapValue, TObjectRemovedEventArgs>
        where TMapKey : class
        where TMapValue : class
        where TObjectRemovedEventArgs : EventArgs
    {
        /// <summary>
        /// Event that is raised when an object has been removed from the <see cref="GameDataMap"/> collection.
        /// </summary>
        public event EventHandler<TObjectRemovedEventArgs> ObjectRemoved;

        /// <summary>
        /// Gets the map object mapping game IDs to game data objects.
        /// </summary>
        public IObjectMapService<TMapKey, TMapValue> GameDataMap
        {
            get;
        }

        /// <summary>
        /// Creates a new game and maps the newly created game object to the unique ID
        /// in the <see cref="GameDataMap"/> map.
        /// </summary>
        /// <param name="turnTime">The amount of time a player has to complete his turn.</param>
        /// <returns>An object containing game and player IDs that are to be transmitted to the players.</returns>
        public Task<CreatedGameData> CreateNewGameAsync(int turnTime);

        /// <summary>
        /// Removes a game from the game data map.
        /// </summary>
        /// <param name="gameID">The ID of the game to remove.</param>
        /// <returns>A task object handling the logic of removing data.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if gameID does not exist in the game data map.
        /// </exception>
        public Task RemoveGameAsync(string gameID);
    }
}
