using System;
using Foundation;
using UIKit;

namespace Mitten.Mobile.iOS
{
    /// <summary>
    /// Represents a cache of UIImage objects.
    /// </summary>
    public class UIImageCache : Cache<UIImage>
    {
        /// <summary>
        /// Initializes a new instance of the UIImageCache class.
        /// </summary>
        /// <param name="cachePercentageSize">The percentage of total physical memoery that the cache will be allowed to grow.</param>
        public UIImageCache(float cachePercentageSize)
            : base(cachePercentageSize)
        {
        }

        /// <summary>
        /// Converts the specified value into an NSObject.
        /// </summary>
        /// <param name="value">A value.</param>
        /// <returns>An NSObject.</returns>
        protected override NSObject ToNSObject(UIImage value)
        {
            return value;
        }

        /// <summary>
        /// Converts the specified NSObject into a value.
        /// </summary>
        /// <param name="obj">An NSObject.</param>
        /// <returns>A value.</returns>
        protected override UIImage FromNSObject(NSObject obj)
        {
            return (UIImage)obj;
        }

        /// <summary>
        /// Calculates the cost of the specified value.
        /// </summary>
        /// <param name="value">A value.</param>
        /// <returns>The value's cost.</returns>
        protected override nuint CalculateCost(UIImage value)
        {
            return (uint)(value.CGImage.BytesPerRow * value.CGImage.Height);
        }
    }
}