using System;
using Foundation;

namespace Mitten.Mobile.iOS
{
    /// <summary>
    /// A cache wrapper for an NSCache instance.
    /// </summary>
    public abstract class Cache<TValue> : ICache<TValue>
        where TValue : class
    {
        private readonly NSCache cache;

        /// <summary>
        /// Initializes a new instance of the Cache class.
        /// </summary>
        /// <param name="cachePercentageSize">The percentage of total physical memoery that the cache will be allowed to grow.</param>
        protected Cache(float cachePercentageSize)
        {
            if (cachePercentageSize >= 1.0f)
            {
                throw new ArgumentOutOfRangeException(nameof(cachePercentageSize), "The percentage size must be less than 1.0f.");
            }

            this.cache = new NSCache();
            this.cache.TotalCostLimit = (nuint)(NSProcessInfo.ProcessInfo.PhysicalMemory * cachePercentageSize);
        }

        /// <summary>
        /// Gets whether or not an image with the specified key exists in the cache.
        /// </summary>
        /// <param name="key">The key for the image.</param>
        /// <returns>True if an image for the key exists, otherwise false.</returns>
        public bool Exists(string key)
        {
            return this.TryGet(key) != null;
        }

        /// <summary>
        /// Adds an item with the specified key to the cache or overwrites an item if one already exists for the provided key.
        /// </summary>
        /// <param name="key">The key for the item.</param>
        /// <param name="value">The item to add.</param>
        public void Put(string key, TValue value)
        {
            Throw.IfArgumentNullOrWhitespace(key, nameof(key));
            this.cache.SetCost(this.ToNSObject(value), new NSString(key), this.CalculateCost(value));
        }

        /// <summary>
        /// Gets the item with the specified key or null if the item does not exist.
        /// </summary>
        /// <param name="key">The key for the item.</param>
        /// <returns>The item or null.</returns>
        public TValue TryGet(string key)
        {
            Throw.IfArgumentNullOrWhitespace(key, nameof(key));

            NSObject item = this.cache.ObjectForKey(new NSString(key));

            return
                item != null
                ? this.FromNSObject(item)
                : null;
        }

        /// <summary>
        /// Removes the item with the specified key if it exists.
        /// </summary>
        /// <param name="key">The key for the item.</param>
        public void Remove(string key)
        {
            Throw.IfArgumentNullOrWhitespace(key, nameof(key));
            this.cache.RemoveObjectForKey(new NSString(key));
        }

        /// <summary>
        /// Clears all the items from the cache.
        /// </summary>
        public void Clear()
        {
            this.cache.RemoveAllObjects();
        }

        /// <summary>
        /// Converts the specified value into an NSObject.
        /// </summary>
        /// <param name="value">A value.</param>
        /// <returns>An NSObject.</returns>
        protected abstract NSObject ToNSObject(TValue value);

        /// <summary>
        /// Converts the specified NSObject into a value.
        /// </summary>
        /// <param name="obj">An NSObject.</param>
        /// <returns>A value.</returns>
        protected abstract TValue FromNSObject(NSObject obj);

        /// <summary>
        /// Calculates the cost of the specified value.
        /// </summary>
        /// <param name="value">A value.</param>
        /// <returns>The value's cost.</returns>
        protected abstract nuint CalculateCost(TValue value);
    }
}