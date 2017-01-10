using System;
using NodaTime;

namespace Mitten.Server
{
    /// <summary>
    /// Contains helper methods for converting date and time instances.
    /// </summary>
    public static class DateTimeConverter
    {
        /// <summary>
        /// Converts a Unit timestamp into a DateTime UTC instance.
        /// </summary>
        /// <param name="secondsSinceUnixEpoch">The number of seconds since the Unix epoch.</param>
        /// <returns>A DateTime instance representing the Unix timestamp.</returns>
        public static DateTime FromUnixTimestamp(int secondsSinceUnixEpoch)
        {
            return Instant.FromSecondsSinceUnixEpoch(secondsSinceUnixEpoch).ToDateTimeUtc();
        }
    }
}
