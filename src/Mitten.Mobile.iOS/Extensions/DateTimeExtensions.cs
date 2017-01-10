using System;
using Foundation;

namespace Mitten.Mobile.iOS.Extensions
{
    /// <summary>
    /// Contains DateTime extensions.
    /// </summary>
    public static class DateTimeExtensions
    {
        private static class Constants
        {
            public static readonly DateTime Reference = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        }

        /// <summary>
        /// Converts a DateTime into a NSDate instance.
        /// </summary>
        /// <param name="dateTime">The date.</param>
        /// <returns>The NS date.</returns>
        public static NSDate ToNSDate(this DateTime dateTime)
        {
            DateTime utcDateTime = dateTime.ToUniversalTime();
            return NSDate.FromTimeIntervalSinceReferenceDate((utcDateTime - Constants.Reference).TotalSeconds);
        }
    }
}
