//-----------------------------------------------------------------------
// <copyright file="GenericMappingService.cs" company="FHWN">
//     Copyright (c) Fachhochschule Wiener Neustadt. All rights reserved.
// </copyright>
// <author>Gregor Faiman.</author>
//-----------------------------------------------------------------------
namespace SharedData.SharedHubData.ConcreteTypes
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SharedData.SharedHubData.Interfaces;

    /// <summary>
    /// This class maps keys to client objects implementing <see cref="IClientContract"/>.
    /// </summary>
    public class GenericMappingService<TKey, TValue> : IObjectMapService<TKey, TValue>
        where TValue : class
        where TKey : IEquatable<TKey>
    {
        private ConcurrentDictionary<TKey, TValue> userMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserConnectionsMapService"/> class.
        /// </summary>
        public GenericMappingService()
        {
            this.userMap = new ConcurrentDictionary<TKey, TValue>();
        }

        /// <summary>
        /// Gets the connection id associated with a user name.
        /// </summary>
        /// <param name="key">The specified username.</param>
        /// <returns>A task which upon completion will contain the connection id in its result..</returns>
        /// <exception cref="KeyNotFoundException">
        /// Is thrown if the specified username was not associated with any entry.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if username is null.
        /// </exception>
        public Task<TValue> GetValueAsync(TKey key)
        {
            return Task.Run(() =>
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key), "Key must not be null.");

                foreach (var item in this.userMap.Keys)
                {
                    if (item.Equals(key))
                        return this.userMap[item];
                }

                throw new KeyNotFoundException("Key was not associated with any value.");
                });
        }

        /// <summary>
        /// Deletes the entry associated with the specified key.
        /// </summary>
        /// <param name="key">The specified key serving as the key.</param>
        /// <returns>A task which upon completion will contain a value indicating whether the entry was successfully deleted.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if key is null.
        /// </exception>
        public Task<bool> TryDeleteEntryAsync(TKey key)
        {
           return Task.Run(() =>
            {
                var success = this.userMap.TryRemove(key, out TValue value);

                return success;
            });
        }

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
        public Task StoreEntryAsync(TKey key, TValue value)
        {
            return Task.Run(() =>
            {
                if (this.userMap.ContainsKey(key))
                    throw new ArgumentException(nameof(key), $"There was already a record with {key} as its key.");

                this.userMap.TryAdd(key, value);
            });
        }

        /// <summary>
        /// Checks whether a record associated with the specified key already exists.
        /// </summary>
        /// <param name="key">The specified username.</param>
        /// <returns>A task which upon completion will contain a value indicating whether an entry associated with the specified username already exists.</returns>
        /// <exception cref="ArgumentNullException">
        /// Is thrown if username is null.
        /// </exception>
        public Task<bool> DoesEntryExistAsync(TKey key)
        {
            return Task.Run(() =>
             {
                 if (key == null)
                     throw new ArgumentNullException(nameof(key), "Key must not be null.");

                 return this.userMap.ContainsKey(key);
             });
        }

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
        public Task<TKey> GetAssociatedKeyAsync(TValue value)
        {
            return Task.Run(() =>
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value), "Value must not be null.");

                foreach (var item in this.userMap.Keys)
                {
                    if (this.userMap[item].Equals(value))
                        return item;
                }

                throw new ArgumentException(nameof(value), "The specified value is not associated with any of the stored keys.");
            });
        }

        /// <summary>
        /// Gets all values in this client map asynchronously.
        /// </summary>
        /// <returns>All values in this map.</returns>
        /// <exception cref="InvalidOperationException">
        /// Is thrown if you call this method while no element are stored inside the map.
        /// </exception>
        public Task<IEnumerable<TValue>> GetAllValuesAsync()
        {
            return Task.Run<IEnumerable<TValue>>(() =>
            {
                return this.userMap.Values;
            });
        }

        /// <summary>
        /// Gets all values except for the value associated with the specified key.
        /// </summary>
        /// <param name="except">The key associated with the value specified to ignore.</param>
        /// <returns>A task object containing the resulting enumerable collection in its result.</returns>
        /// <exception cref="ArgumentException">
        /// Might be thrown if the specified key is invalid.
        /// </exception>
        public Task<IEnumerable<TValue>> GetAllValuesExceptAsync(TKey except)
        {
            return Task.Run<IEnumerable<TValue>>(() =>
            {
                var result = new List<TValue>();

                foreach (var item in this.userMap.Keys)
                {
                    if (item.Equals(except))
                        continue;

                    result.Add(this.userMap[item]);
                }

                return result;
            });
        }

        /// <summary>
        /// Tries to get the value from the map.
        /// </summary>
        /// <param name="key">The key associated with the requested value.</param>
        /// <param name="value">The received value.</param>
        /// <returns>A task containing a value indicating whether the retrieval was successful in its result.</returns>
        public Task<bool> TryGetValueAsync(TKey key, out TValue value)
        {
            foreach (var item in this.userMap.Keys)
            {
                if (item.Equals(key))
                {
                    value = this.userMap[item];
                    return Task.FromResult(true);
                }
            }

            value = null;
            return Task.FromResult(false);
        }
    }
}
