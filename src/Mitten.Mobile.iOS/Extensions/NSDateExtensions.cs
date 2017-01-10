using System;
using Foundation;

namespace Mitten.Mobile.iOS.Extensions
{
    /// <summary>
    /// Contains NSDate extensions.
    /// </summary>
    public static class NSDateExtensions
    {
        private static class Constants
        {
            public static readonly DateTime Reference = new DateTime(2001, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        }

        /// <summary>
        /// Converts a NSDate into a DateTime instance.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The date time.</returns>
        public static DateTime ToDateTimeUTC(this NSDate date)
        {
            return Constants.Reference.AddSeconds(date.SecondsSinceReferenceDate);
        }
    }
}
