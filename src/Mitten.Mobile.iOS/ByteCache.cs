using System;
using System.Runtime.InteropServices;
using Foundation;

namespace Mitten.Mobile.iOS
{
    /// <summary>
    /// Represents a cache of byte arrays.
    /// </summary>
    public class ByteCache : Cache<byte[]>
    {
        /// <summary>
        /// Initializes a new instance of the ByteCache class.
        /// </summary>
        /// <param name="cachePercentageSize">The percentage of total physical memoery that the cache will be allowed to grow.</param>
        public ByteCache(float cachePercentageSize)
            : base(cachePercentageSize)
        {
        }

        /// <summary>
        /// Converts the specified value into an NSObject.
        /// </summary>
        /// <param name="value">A value.</param>
        /// <returns>An NSObject.</returns>
        protected override NSObject ToNSObject(byte[] value)
        {
            return NSData.FromArray(value);
        }

        /// <summary>
        /// Converts the specified NSObject into a value.
        /// </summary>
        /// <param name="obj">An NSObject.</param>
        /// <returns>A value.</returns>
        protected override byte[] FromNSObject(NSObject obj)
        {
            // TODO: is it possible to get a reference to the array instead of copying?

            NSData data = (NSData)obj;
            byte[] bytes = new byte[data.Length];

            Marshal.Copy(
                data.Bytes,
                bytes,
                0,
                Convert.ToInt32(data.Length));

            return bytes;
        }

        /// <summary>
        /// Calculates the cost of the specified value.
        /// </summary>
        /// <param name="value">A value.</param>
        /// <returns>The value's cost.</returns>
        protected override nuint CalculateCost(byte[] value)
        {
            return (uint)value.Length;
        }
    }
}