namespace Mitten.Mobile
{
    /// <summary>
    /// Defines an interface for a basic cache. 
    /// </summary>
    public interface ICache<TValue>
        where TValue : class
    {
        /// <summary>
        /// Gets whether or not an item with the specified key exists in the cache.
        /// </summary>
        /// <param name="key">The key for the item.</param>
        /// <returns>True if an item for the key exists, otherwise false.</returns>
        bool Exists(string key);

        /// <summary>
        /// Adds an item with the specified key to the cache or overwrites an item if one already exists for the provided key.
        /// </summary>
        /// <param name="key">The key for the item.</param>
        /// <param name="value">The item to add.</param>
        void Put(string key, TValue value);

        /// <summary>
        /// Gets the item with the specified key or null if the item does not exist.
        /// </summary>
        /// <param name="key">The key for the item.</param>
        /// <returns>The item or null.</returns>
        TValue TryGet(string key);

        /// <summary>
        /// Removes the item with the specified key if it exists.
        /// </summary>
        /// <param name="key">The key for the item.</param>
        void Remove(string key);

        /// <summary>
        /// Clears all the items from the cache.
        /// </summary>
        void Clear();
    }
}