//-----------------------------------------------------------------------
// <copyright file="IUserMap.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SharedData.SharedHubData.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a mapping service, capable of mapping keys to user objects.
    /// </summary>
    public interface IObjectMapService<TKey, TValue> where TValue : class
    {
        /// <summary>
        /// Gets value associated with a specified key.
        /// </summary>
        /// <param name="key">The specified key.</param>
        /// <returns>A task which upon completion will contain the value in its result.</returns>
        /// <exception cref="ArgumentException">
        /// Is thrown if the specified key was not associated with any entry.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if key is null.
        /// </exception>
        Task<TValue> GetValueAsync(TKey key);

        /// <summary>
        /// Deletes the entry associated with the specified key.
        /// </summary>
        /// <param name="key">The specified key serving as the key.</param>
        /// <returns>A task which upon completion will contain a value indicating whether the entry was successfully deleted.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if key is null.
        /// </exception>
        Task<bool> TryDeleteEntryAsync(TKey key);

        /// <summary>
        /// Stores the specified value and associates it with the specified key.
        /// </summary>
        /// <param name="key">The key with which to access the value.</param>
        /// <param name="value">The value that is being stored.</param>
        /// <returns>A task which will work through the logic of storing an entry in the user map.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if either key or value are null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Is thrown if there is already an entry being stored with the specified key.
        /// </exception>
        Task StoreEntryAsync(TKey key, TValue value);

        /// <summary>
        /// Checks whether a record associated with the specified username already exists.
        /// </summary>
        /// <param name="key">The specified username.</param>
        /// <returns>A task which upon completion will contain a value indicating whether an entry associated with the specified username already exists.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if username is null.
        /// </exception>
        Task<bool> DoesEntryExistAsync(TKey key);

        /// <summary>
        /// Gets the key associated with the specified value.
        /// </summary>
        /// <param name="value">The value for which the key is requested.</param>
        /// <returns>A task which upon completion will contain the associated key.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if value is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Is thrown if the specified value was not associated with any key.
        /// </exception>
        Task<TKey> GetAssociatedKeyAsync(TValue value);

        /// <summary>
        /// Gets all values in this client map asynchronously.
        /// </summary>
        /// <returns>A task object containing the resulting enumerable collection in its result.</returns>
        /// <exception cref="InvalidOperationException">
        /// Is thrown if you call this method while no element are stored inside the map.
        /// </exception>
        Task<IEnumerable<TValue>> GetAllValuesAsync();

        /// <summary>
        /// Gets all values except for the value associated with the specified key.
        /// </summary>
        /// <param name="except">The key associated with the value specified to ignore.</param>
        /// <returns>A task object containing the resulting enumerable collection in its result.</returns>
        /// <exception cref="ArgumentException">
        /// Might be thrown if the specified key is invalid.
        /// </exception>
        Task<IEnumerable<TValue>> GetAllValuesExceptAsync(TKey except);

        /// <summary>
        /// Tries to get the value from the map.
        /// </summary>
        /// <param name="key">The key associated with the requested value.</param>
        /// <param name="value">The received value.</param>
        /// <returns>A task containing a value indicating whether the retrieval was successful in its result.</returns>
        Task<bool> TryGetValueAsync(TKey key, out TValue value);
    }
}
