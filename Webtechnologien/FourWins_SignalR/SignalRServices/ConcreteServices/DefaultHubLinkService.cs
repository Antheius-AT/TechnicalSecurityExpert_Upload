//-----------------------------------------------------------------------
// <copyright file="DefaultHubLink.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SignalRServices.ConcreteServices
{
    using System;
    using System.Threading.Tasks;
    using SharedData.SharedHubData.Interfaces;
    using SignalRServices.EventArgs;
    using SignalRServices.Interfaces.ServiceInterfaces.SharedServices;
    using SignalRServices.ServiceData;

    /// <summary>
    /// Default implementation of the link between game hub and lobby hub.
    /// </summary>
    public class DefaultHubLinkService : IHubLinkService<string, GameData, ObjectRemovedEventArgs>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameData"/> class.
        /// </summary>
        /// <param name="gameMap">The map mapping a game data object to a game ID.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if game map is null.
        /// </exception>
        public DefaultHubLinkService(IObjectMapService<string, GameData> gameMap)
        {
            if (gameMap == null)
                throw new ArgumentNullException(nameof(gameMap), "Game map must not be null.");

            this.GameDataMap = gameMap;
        }

        /// <summary>
        /// This event notifies the subscribers that an object has been removed from the <see cref="GameDataMap"/> collection.
        /// </summary>
        public event EventHandler<ObjectRemovedEventArgs> ObjectRemoved;

        /// <summary>
        /// Gets the map object mapping game IDs to game data objects.
        /// </summary>
        public IObjectMapService<string, GameData> GameDataMap
        {
            get;
        }

        /// <summary>
        /// Creates a new game and maps the newly created game object to the unique ID
        /// in the <see cref="GameDataMap"/> map.
        /// </summary>
        /// <param name="turnTime">The amount of time a player has to complete his turn.</param>
        /// <returns>An object containing game and player IDs that are to be transmitted to the players.</returns>
        public Task<CreatedGameData> CreateNewGameAsync(int turnTime)
        {
            return Task.Run(async () =>
            {
                var newGame = new GameData(turnTime);

                var gameID = Guid.NewGuid().ToString();
                await this.GameDataMap.StoreEntryAsync(gameID, newGame);

                return new CreatedGameData(gameID, newGame.PlayerID);
            });
        }

        /// <summary>
        /// Removes a game from the game data map.
        /// </summary>
        /// <param name="gameID">The ID of the game data to remove.</param>
        /// <returns>A task object handling the logic of removing data.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if gameID does not exist in the game data map.
        /// </exception>
        public Task RemoveGameAsync(string gameID)
        {
            if (!GameDataMap.DoesEntryExistAsync(gameID).Result)
                throw new ArgumentNullException(nameof(gameID), $"No entry with {gameID} found.");

            return Task.Run(async () =>
            {
                await this.GameDataMap.TryDeleteEntryAsync(gameID);
                this.RaiseObjectRemoved(gameID);
            });
        }

        /// <summary>
        /// Raises the <see cref="ObjectRemoved"/> event to notify subscribers that an object was removed.
        /// </summary>
        /// <param name="data">The object that was removed.</param>
        protected virtual void RaiseObjectRemoved(string gameID)
        {
            this.ObjectRemoved?.Invoke(this, new ObjectRemovedEventArgs(gameID));
        }
    }
}
